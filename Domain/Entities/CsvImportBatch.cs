using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class CsvImportBatch : BaseEntity, ITenantScoped
    {
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        public Guid SalesChannelId { get; set; }
        public Guid WarehouseId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // Pending, Processing, Completed, Failed

        public int TotalRows { get; set; }
        public int SuccessRows { get; set; }
        public int FailedRows { get; set; }
        public int SkippedRows { get; set; }

        public DateTime? CompletedAt { get; set; }

        [MaxLength(2000)]
        public string? ErrorSummary { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public SalesChannel SalesChannel { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public ICollection<CsvImportRow> Rows { get; set; } = new List<CsvImportRow>();
    }
}
