using Application.DTOs.Export;
using Application.Interfaces;
using Domain.Enums;
using System.Text;

namespace Application.UseCases.Export
{
    public class ExportUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IJournalEntryRepository _journalEntryRepository;
        private readonly IStockMovementRepository _stockMovementRepository;
        private readonly IProductRepository _productRepository;

        public ExportUseCase(
            IInvoiceRepository invoiceRepository,
            IJournalEntryRepository journalEntryRepository,
            IStockMovementRepository stockMovementRepository,
            IProductRepository productRepository)
        {
            _invoiceRepository = invoiceRepository;
            _journalEntryRepository = journalEntryRepository;
            _stockMovementRepository = stockMovementRepository;
            _productRepository = productRepository;
        }

        public async Task<(byte[] Content, string FileName)> ExportAsync(ExportRequest request)
        {
            return request.EntityType.ToLower() switch
            {
                "invoices" => await ExportInvoicesAsync(request),
                "journal-entries" => await ExportJournalEntriesAsync(request),
                "stock-movements" => await ExportStockMovementsAsync(request),
                "products" => await ExportProductsAsync(request),
                _ => throw new InvalidOperationException($"Unsupported export type: {request.EntityType}")
            };
        }

        private async Task<(byte[] Content, string FileName)> ExportInvoicesAsync(ExportRequest request)
        {
            var paged = await _invoiceRepository.GetPagedAsync(
                new Application.DTOs.Common.PagedRequest { Page = 1, PageSize = 100 }, request.Status);

            var sb = new StringBuilder();
            sb.AppendLine("InvoiceNumber,Status,CounterpartyName,InvoiceDate,DueDate,SubTotal,DiscountAmount,TaxAmount,GrandTotal,CreatedAt");

            foreach (var inv in paged.Items)
            {
                sb.AppendLine($"{Escape(inv.InvoiceNumber)},{inv.Status},{Escape(inv.CounterpartyName)},{inv.InvoiceDate:yyyy-MM-dd},{inv.DueDate:yyyy-MM-dd},{inv.SubTotal},{inv.DiscountAmount},{inv.TaxAmount},{inv.GrandTotal},{inv.CreatedAt:yyyy-MM-dd HH:mm}");
            }

            var fileName = $"invoices-{DateTime.UtcNow:yyyyMMdd-HHmm}.csv";
            return (Encoding.UTF8.GetBytes(sb.ToString()), fileName);
        }

        private async Task<(byte[] Content, string FileName)> ExportJournalEntriesAsync(ExportRequest request)
        {
            var paged = await _journalEntryRepository.GetPagedAsync(
                new Application.DTOs.Common.PagedRequest { Page = 1, PageSize = 100 }, request.Status);

            var sb = new StringBuilder();
            sb.AppendLine("EntryNumber,EntryDate,Description,Status,TotalDebit,TotalCredit,PostedAt");

            foreach (var entry in paged.Items)
            {
                sb.AppendLine($"{entry.EntryNumber},{entry.EntryDate:yyyy-MM-dd},{Escape(entry.Description)},{entry.Status},{entry.TotalDebit},{entry.TotalCredit},{entry.PostedAt:yyyy-MM-dd HH:mm}");
            }

            var fileName = $"journal-entries-{DateTime.UtcNow:yyyyMMdd-HHmm}.csv";
            return (Encoding.UTF8.GetBytes(sb.ToString()), fileName);
        }

        private async Task<(byte[] Content, string FileName)> ExportStockMovementsAsync(ExportRequest request)
        {
            var paged = await _stockMovementRepository.GetPagedAsync(
                new Application.DTOs.Common.PagedRequest { Page = 1, PageSize = 100 },
                movementType: request.Status);

            var sb = new StringBuilder();
            sb.AppendLine("MovementType,Reason,SKU,WarehouseName,Quantity,QuantityBefore,QuantityAfter,ReferenceDocNumber,CreatedAt");

            foreach (var m in paged.Items)
            {
                sb.AppendLine($"{m.MovementType},{m.Reason},{Escape(m.ProductVariant?.Sku)},{Escape(m.Warehouse?.Name)},{m.Quantity},{m.QuantityBefore},{m.QuantityAfter},{Escape(m.ReferenceDocumentNumber)},{m.CreatedAt:yyyy-MM-dd HH:mm}");
            }

            var fileName = $"stock-movements-{DateTime.UtcNow:yyyyMMdd-HHmm}.csv";
            return (Encoding.UTF8.GetBytes(sb.ToString()), fileName);
        }

        private async Task<(byte[] Content, string FileName)> ExportProductsAsync(ExportRequest request)
        {
            var paged = await _productRepository.GetPagedAsync(
                new Application.DTOs.Common.PagedRequest { Page = 1, PageSize = 100 });

            var sb = new StringBuilder();
            sb.AppendLine("Name,Category,Brand,Status,IsActive,CreatedAt");

            foreach (var p in paged.Items)
            {
                sb.AppendLine($"{Escape(p.Name)},{Escape(p.Category?.Name)},{Escape(p.Brand?.Name)},{p.Status},{p.IsActive},{p.CreatedAt:yyyy-MM-dd}");
            }

            var fileName = $"products-{DateTime.UtcNow:yyyyMMdd-HHmm}.csv";
            return (Encoding.UTF8.GetBytes(sb.ToString()), fileName);
        }

        private static string Escape(string? value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
    }
}
