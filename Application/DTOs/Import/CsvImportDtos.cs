using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Import
{
    public class SalesChannelDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal PlatformFeePercent { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateSalesChannelRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public decimal PlatformFeePercent { get; set; }
    }

    public class CsvImportBatchDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public Guid SalesChannelId { get; set; }
        public string SalesChannelName { get; set; } = string.Empty;
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TotalRows { get; set; }
        public int SuccessRows { get; set; }
        public int FailedRows { get; set; }
        public int SkippedRows { get; set; }
        public string? ErrorSummary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class CsvImportRowDto
    {
        public Guid Id { get; set; }
        public int RowNumber { get; set; }
        public string? OrderNumber { get; set; }
        public string? Sku { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PlatformFee { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid? MatchedProductVariantId { get; set; }
        public string? MatchedSku { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class CsvImportBatchDetailDto : CsvImportBatchDto
    {
        public List<CsvImportRowDto> Rows { get; set; } = new();
    }

    public class CsvColumnMapping
    {
        [Required]
        public string OrderNumberColumn { get; set; } = string.Empty;
        [Required]
        public string SkuColumn { get; set; } = string.Empty;
        [Required]
        public string QuantityColumn { get; set; } = string.Empty;
        [Required]
        public string UnitPriceColumn { get; set; } = string.Empty;
        public string? TotalPriceColumn { get; set; }
        public string? ProductNameColumn { get; set; }
        public string? OrderDateColumn { get; set; }
        public string? PlatformFeeColumn { get; set; }
    }

    public class StartImportRequest
    {
        [Required]
        public Guid SalesChannelId { get; set; }
        [Required]
        public Guid WarehouseId { get; set; }
        [Required]
        public CsvColumnMapping ColumnMapping { get; set; } = new();
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
        public List<CsvImportRowDto> FailedRowDetails { get; set; } = new();
    }
}
