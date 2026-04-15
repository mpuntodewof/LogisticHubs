namespace Application.DTOs.Export
{
    public class ExportRequest
    {
        public string EntityType { get; set; } = string.Empty; // invoices, journal-entries, stock-movements, products
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
    }
}
