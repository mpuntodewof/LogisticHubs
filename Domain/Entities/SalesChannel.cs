using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class SalesChannel : BaseEntity, ITenantScoped
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public decimal PlatformFeePercent { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public ICollection<CsvImportBatch> ImportBatches { get; set; } = new List<CsvImportBatch>();
    }
}
