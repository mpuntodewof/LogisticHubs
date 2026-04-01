using Application.DTOs.Common;
using Application.DTOs.Tax;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Tax
{
    public class InvoiceUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ISalesOrderRepository _salesOrderRepository;
        private readonly ITaxRateRepository _taxRateRepository;
        private readonly IPaymentTermRepository _paymentTermRepository;

        public InvoiceUseCase(
            IInvoiceRepository invoiceRepository,
            ISalesOrderRepository salesOrderRepository,
            ITaxRateRepository taxRateRepository,
            IPaymentTermRepository paymentTermRepository)
        {
            _invoiceRepository = invoiceRepository;
            _salesOrderRepository = salesOrderRepository;
            _taxRateRepository = taxRateRepository;
            _paymentTermRepository = paymentTermRepository;
        }

        public async Task<PagedResult<InvoiceDto>> GetPagedAsync(PagedRequest request, string? status = null)
        {
            var result = await _invoiceRepository.GetPagedAsync(request, status);

            return new PagedResult<InvoiceDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<InvoiceDetailDto?> GetByIdAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetDetailByIdAsync(id);
            return invoice == null ? null : MapToDetailDto(invoice);
        }

        public async Task<InvoiceDto> CreateFromSalesOrderAsync(CreateInvoiceFromOrderRequest request)
        {
            var order = await _salesOrderRepository.GetDetailByIdAsync(request.SalesOrderId)
                ?? throw new InvalidOperationException("Sales order not found.");

            if (order.Status != SalesOrderStatus.Confirmed.ToString())
                throw new InvalidOperationException("Only confirmed sales orders can be invoiced.");

            // Check if invoice already exists for this order
            var existing = await _invoiceRepository.GetBySalesOrderIdAsync(request.SalesOrderId);
            if (existing != null)
                throw new InvalidOperationException("An invoice already exists for this sales order.");

            var invoiceNumber = await GenerateInvoiceNumberAsync();
            var invoiceDate = DateTime.UtcNow;

            // Determine due date
            DateTime dueDate;
            if (request.PaymentTermId.HasValue)
            {
                var paymentTerm = await _paymentTermRepository.GetByIdAsync(request.PaymentTermId.Value)
                    ?? throw new InvalidOperationException("Payment term not found.");
                dueDate = invoiceDate.AddDays(paymentTerm.DueDays);
            }
            else
            {
                dueDate = invoiceDate.AddDays(30);
            }

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = invoiceNumber,
                TaxInvoiceNumber = request.TaxInvoiceNumber,
                SalesOrderId = order.Id,
                CustomerId = order.CustomerId,
                BranchId = order.BranchId,
                PaymentTermId = request.PaymentTermId,
                Status = InvoiceStatus.Draft.ToString(),
                InvoiceDate = invoiceDate,
                DueDate = dueDate,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            var items = new List<InvoiceItem>();
            decimal subTotal = 0;
            decimal totalDiscount = 0;
            decimal totalTax = 0;

            foreach (var orderItem in order.Items)
            {
                var lineSubTotal = orderItem.UnitPrice * orderItem.Quantity;
                var lineDiscount = orderItem.DiscountAmount;
                var taxableAmount = lineSubTotal - lineDiscount;

                // Lookup active tax rates for this product
                var taxRates = await _taxRateRepository.GetActiveByProductIdAsync(orderItem.ProductVariantId);
                var taxRate = taxRates.FirstOrDefault();

                var taxRateValue = taxRate?.Rate ?? 0m;
                var taxAmount = taxableAmount * taxRateValue;
                var lineTotal = taxableAmount + taxAmount;

                var invoiceItem = new InvoiceItem
                {
                    Id = Guid.NewGuid(),
                    InvoiceId = invoice.Id,
                    SalesOrderItemId = orderItem.Id,
                    ProductVariantId = orderItem.ProductVariantId,
                    ProductName = orderItem.ProductName,
                    VariantName = orderItem.VariantName,
                    Sku = orderItem.Sku,
                    Quantity = orderItem.Quantity,
                    UnitPrice = orderItem.UnitPrice,
                    DiscountAmount = lineDiscount,
                    TaxRateId = taxRate?.Id,
                    TaxRateValue = taxRateValue,
                    TaxAmount = taxAmount,
                    LineTotal = lineTotal,
                    CreatedAt = DateTime.UtcNow
                };

                items.Add(invoiceItem);

                subTotal += lineSubTotal;
                totalDiscount += lineDiscount;
                totalTax += taxAmount;
            }

            invoice.Items = items;
            invoice.SubTotal = subTotal;
            invoice.DiscountAmount = totalDiscount;
            invoice.TaxableAmount = subTotal - totalDiscount;
            invoice.TaxAmount = totalTax;
            invoice.GrandTotal = invoice.TaxableAmount + totalTax;

            var created = await _invoiceRepository.CreateAsync(invoice);
            return MapToDto(created);
        }

        public async Task IssueAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Invoice not found.");

            if (invoice.Status != InvoiceStatus.Draft.ToString())
                throw new InvalidOperationException("Only draft invoices can be issued.");

            invoice.Status = InvoiceStatus.Issued.ToString();
            invoice.IssuedAt = DateTime.UtcNow;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _invoiceRepository.UpdateAsync(invoice);
        }

        public async Task AssignTaxInvoiceNumberAsync(Guid id, AssignTaxInvoiceNumberRequest request)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Invoice not found.");

            invoice.TaxInvoiceNumber = request.TaxInvoiceNumber;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _invoiceRepository.UpdateAsync(invoice);
        }

        public async Task MarkPaidAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Invoice not found.");

            if (invoice.Status != InvoiceStatus.Issued.ToString())
                throw new InvalidOperationException("Only issued invoices can be marked as paid.");

            invoice.Status = InvoiceStatus.Paid.ToString();
            invoice.PaidAt = DateTime.UtcNow;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _invoiceRepository.UpdateAsync(invoice);
        }

        public async Task CancelAsync(Guid id, string reason)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Invoice not found.");

            if (invoice.Status != InvoiceStatus.Draft.ToString() && invoice.Status != InvoiceStatus.Issued.ToString())
                throw new InvalidOperationException("Only draft or issued invoices can be cancelled.");

            invoice.Status = InvoiceStatus.Cancelled.ToString();
            invoice.CancelledAt = DateTime.UtcNow;
            invoice.CancellationReason = reason;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _invoiceRepository.UpdateAsync(invoice);
        }

        public async Task DeleteAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Invoice not found.");

            if (invoice.Status != InvoiceStatus.Draft.ToString())
                throw new InvalidOperationException("Only draft invoices can be deleted.");

            await _invoiceRepository.DeleteAsync(invoice);
        }

        private async Task<string> GenerateInvoiceNumberAsync()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string invoiceNumber;

            do
            {
                var suffix = new string(Enumerable.Range(0, 4).Select(_ => chars[random.Next(chars.Length)]).ToArray());
                invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{suffix}";
            }
            while (await _invoiceRepository.InvoiceNumberExistsAsync(invoiceNumber));

            return invoiceNumber;
        }

        private static InvoiceDto MapToDto(Invoice i) => new()
        {
            Id = i.Id,
            InvoiceNumber = i.InvoiceNumber,
            TaxInvoiceNumber = i.TaxInvoiceNumber,
            SalesOrderId = i.SalesOrderId,
            SalesOrderNumber = i.SalesOrder?.OrderNumber,
            CustomerId = i.CustomerId,
            CustomerName = i.Customer?.Name,
            BranchId = i.BranchId,
            BranchName = i.Branch?.Name,
            Status = i.Status,
            InvoiceDate = i.InvoiceDate,
            DueDate = i.DueDate,
            SubTotal = i.SubTotal,
            DiscountAmount = i.DiscountAmount,
            TaxableAmount = i.TaxableAmount,
            TaxAmount = i.TaxAmount,
            GrandTotal = i.GrandTotal,
            CreatedAt = i.CreatedAt
        };

        private static InvoiceDetailDto MapToDetailDto(Invoice i) => new()
        {
            Id = i.Id,
            InvoiceNumber = i.InvoiceNumber,
            TaxInvoiceNumber = i.TaxInvoiceNumber,
            SalesOrderId = i.SalesOrderId,
            SalesOrderNumber = i.SalesOrder?.OrderNumber,
            CustomerId = i.CustomerId,
            CustomerName = i.Customer?.Name,
            BranchId = i.BranchId,
            BranchName = i.Branch?.Name,
            Status = i.Status,
            InvoiceDate = i.InvoiceDate,
            DueDate = i.DueDate,
            SubTotal = i.SubTotal,
            DiscountAmount = i.DiscountAmount,
            TaxableAmount = i.TaxableAmount,
            TaxAmount = i.TaxAmount,
            GrandTotal = i.GrandTotal,
            CreatedAt = i.CreatedAt,
            Items = i.Items?.Select(MapItemToDto).ToList() ?? new List<InvoiceItemDto>(),
            PaymentTermName = i.PaymentTerm?.Name,
            Notes = i.Notes,
            IssuedAt = i.IssuedAt,
            PaidAt = i.PaidAt
        };

        private static InvoiceItemDto MapItemToDto(InvoiceItem item) => new()
        {
            Id = item.Id,
            ProductVariantId = item.ProductVariantId,
            ProductName = item.ProductName,
            VariantName = item.VariantName,
            Sku = item.Sku,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            DiscountAmount = item.DiscountAmount,
            TaxRateName = item.TaxRateEntity?.Name,
            TaxRateValue = item.TaxRateValue,
            TaxAmount = item.TaxAmount,
            LineTotal = item.LineTotal
        };
    }
}
