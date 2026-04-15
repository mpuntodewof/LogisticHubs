using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class CsvImportRow : BaseEntity, ITenantScoped
    {
        public Guid CsvImportBatchId { get; set; }
        public int RowNumber { get; set; }

        [MaxLength(100)]
        public string? OrderNumber { get; set; }

        [MaxLength(100)]
        public string? Sku { get; set; }

        [MaxLength(500)]
        public string? ProductName { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PlatformFee { get; set; }

        public DateTime? OrderDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // Matched, Unmatched, Skipped, Error

        public Guid? MatchedProductVariantId { get; set; }
        public Guid? StockMovementId { get; set; }

        [MaxLength(1000)]
        public string? ErrorMessage { get; set; }

        [MaxLength(4000)]
        public string? RawRowJson { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public CsvImportBatch Batch { get; set; } = null!;
        public ProductVariant? MatchedProductVariant { get; set; }
        public StockMovement? StockMovement { get; set; }
    }
}
