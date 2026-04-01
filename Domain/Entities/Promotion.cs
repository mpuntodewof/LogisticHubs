using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Promotion : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Code { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required, MaxLength(50)]
        public string PromotionType { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Draft";

        [MaxLength(50)]
        public string? DiscountType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxDiscountAmount { get; set; }

        public int? BuyQuantity { get; set; }
        public int? GetQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinOrderAmount { get; set; }

        public int? MaxUsageCount { get; set; }
        public int MaxUsagePerCustomer { get; set; } = 1;
        public int CurrentUsageCount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsStackable { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; } = true;

        public Guid TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public ICollection<PromotionRule> Rules { get; set; } = new List<PromotionRule>();
        public ICollection<PromotionProduct> Products { get; set; } = new List<PromotionProduct>();
        public ICollection<PromotionUsage> Usages { get; set; } = new List<PromotionUsage>();
    }
}
