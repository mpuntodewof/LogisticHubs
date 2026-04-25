namespace BlazorApp.Client.Models
{
    public class SalesChannelDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal PlatformFeePercent { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSalesChannelRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal PlatformFeePercent { get; set; }
    }

    public class ImportSummaryDto
    {
        public Guid BatchId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalRows { get; set; }
        public int SuccessRows { get; set; }
        public int FailedRows { get; set; }
        public int SkippedRows { get; set; }
        public int DuplicateRows { get; set; }
        public List<ImportRowDto> FailedRowDetails { get; set; } = new();
    }

    public class ImportRowDto
    {
        public int RowNumber { get; set; }
        public string? OrderNumber { get; set; }
        public string? Sku { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }

    public class ImportBatchDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string SalesChannelName { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TotalRows { get; set; }
        public int SuccessRows { get; set; }
        public int FailedRows { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ── Initial Stock Import ────────────────────────────────────────────────

    public class InitialStockResultDto
    {
        public int TotalRows { get; set; }
        public int SuccessRows { get; set; }
        public int FailedRows { get; set; }
        public int SkippedRows { get; set; }
        public List<InitialStockRowDto> FailedRowDetails { get; set; } = new();
    }

    public class InitialStockRowDto
    {
        public int RowNumber { get; set; }
        public string? Sku { get; set; }
        public int Quantity { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
