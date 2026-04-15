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
        private readonly ITaxRateRepository _taxRateRepository;
        private readonly IPaymentTermRepository _paymentTermRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceUseCase(
            IInvoiceRepository invoiceRepository,
            ITaxRateRepository taxRateRepository,
            IPaymentTermRepository paymentTermRepository,
            ITransactionManager transactionManager,
            IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _taxRateRepository = taxRateRepository;
            _paymentTermRepository = paymentTermRepository;
            _transactionManager = transactionManager;
            _unitOfWork = unitOfWork;
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

        public async Task<InvoiceDto> CreateAsync(CreateInvoiceRequest request)
        {
            if (request.Items == null || request.Items.Count == 0)
                throw new InvalidOperationException("Invoice must have at least one item.");

            await _transactionManager.BeginTransactionAsync();
            try
            {
                var invoiceNumber = await GenerateInvoiceNumberAsync();
                var invoiceDate = DateTime.UtcNow;

                DateTime dueDate;
                if (request.DueDate.HasValue)
                {
                    dueDate = request.DueDate.Value;
                }
                else if (request.PaymentTermId.HasValue)
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
                    ReferenceDocumentType = request.ReferenceDocumentType,
                    ReferenceDocumentId = request.ReferenceDocumentId,
                    ReferenceDocumentNumber = request.ReferenceDocumentNumber,
                    CounterpartyName = request.CounterpartyName,
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

                foreach (var requestItem in request.Items)
                {
                    var lineSubTotal = requestItem.UnitPrice * requestItem.Quantity;
                    var lineDiscount = requestItem.DiscountAmount;
                    var taxableAmount = lineSubTotal - lineDiscount;

                    var taxRates = await _taxRateRepository.GetActiveByProductIdAsync(requestItem.ProductVariantId);
                    var taxRate = taxRates.FirstOrDefault();

                    var taxRateValue = taxRate?.Rate ?? 0m;
                    var taxAmount = taxableAmount * taxRateValue;
                    var lineTotal = taxableAmount + taxAmount;

                    var invoiceItem = new InvoiceItem
                    {
                        Id = Guid.NewGuid(),
                        InvoiceId = invoice.Id,
                        ProductVariantId = requestItem.ProductVariantId,
                        ProductName = requestItem.ProductName,
                        VariantName = requestItem.VariantName,
                        Sku = requestItem.Sku,
                        Quantity = requestItem.Quantity,
                        UnitPrice = requestItem.UnitPrice,
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

                await _unitOfWork.SaveChangesAsync();
                await _transactionManager.CommitAsync();
                return MapToDto(created);
            }
            catch
            {
                await _transactionManager.RollbackAsync();
                throw;
            }
        }

        public async Task IssueAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Invoice not found.");

            invoice.Issue();

            await _invoiceRepository.UpdateAsync(invoice);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AssignTaxInvoiceNumberAsync(Guid id, AssignTaxInvoiceNumberRequest request)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Invoice not found.");

            invoice.TaxInvoiceNumber = request.TaxInvoiceNumber;
            invoice.UpdatedAt = DateTime.UtcNow;

            await _invoiceRepository.UpdateAsync(invoice);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task MarkPaidAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Invoice not found.");

            invoice.MarkPaid();

            await _invoiceRepository.UpdateAsync(invoice);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CancelAsync(Guid id, string reason)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Invoice not found.");

            invoice.Cancel(reason);

            await _invoiceRepository.UpdateAsync(invoice);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Invoice not found.");

            invoice.EnsureCanDelete();

            await _invoiceRepository.DeleteAsync(invoice);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<string> GenerateInvoiceNumberAsync()
        {
            var prefix = $"INV-{DateTime.UtcNow:yyyyMMdd}-";
            var counter = 1;
            string invoiceNumber;

            do
            {
                invoiceNumber = $"{prefix}{counter:D4}";
                counter++;
            }
            while (await _invoiceRepository.InvoiceNumberExistsAsync(invoiceNumber));

            return invoiceNumber;
        }

        private static InvoiceDto MapToDto(Invoice i) => new()
        {
            Id = i.Id,
            InvoiceNumber = i.InvoiceNumber,
            TaxInvoiceNumber = i.TaxInvoiceNumber,
            ReferenceDocumentType = i.ReferenceDocumentType,
            ReferenceDocumentId = i.ReferenceDocumentId,
            ReferenceDocumentNumber = i.ReferenceDocumentNumber,
            CounterpartyName = i.CounterpartyName,
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
            ReferenceDocumentType = i.ReferenceDocumentType,
            ReferenceDocumentId = i.ReferenceDocumentId,
            ReferenceDocumentNumber = i.ReferenceDocumentNumber,
            CounterpartyName = i.CounterpartyName,
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
