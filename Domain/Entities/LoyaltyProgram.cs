using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class LoyaltyProgram : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PointsPerIdrSpent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RedemptionRateIdr { get; set; }

        public int MinRedemptionPoints { get; set; } = 100;
        public int? PointExpiryDays { get; set; }
        public bool IsActive { get; set; } = true;

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Active";

        public Guid TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public ICollection<LoyaltyTier> Tiers { get; set; } = new List<LoyaltyTier>();
        public ICollection<LoyaltyMembership> Memberships { get; set; } = new List<LoyaltyMembership>();
    }
}
