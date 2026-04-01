using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class LoyaltyTier : BaseEntity, ITenantScoped
    {
        public Guid LoyaltyProgramId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int MinPointsThreshold { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal PointsMultiplier { get; set; } = 1.0m;

        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercentage { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public LoyaltyProgram LoyaltyProgram { get; set; } = null!;
        public ICollection<LoyaltyMembership> Memberships { get; set; } = new List<LoyaltyMembership>();
    }
}
