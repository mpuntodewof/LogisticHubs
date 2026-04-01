using Domain.Interfaces;

namespace Domain.Entities
{
    public class LoyaltyMembership : BaseEntity, ITenantScoped
    {
        public Guid LoyaltyProgramId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? CurrentTierId { get; set; }

        public int AvailablePoints { get; set; }
        public int LifetimePoints { get; set; }
        public int TotalRedeemed { get; set; }

        public DateTime JoinedAt { get; set; }
        public DateTime? LastActivityAt { get; set; }
        public bool IsActive { get; set; } = true;

        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public LoyaltyProgram LoyaltyProgram { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public LoyaltyTier? CurrentTier { get; set; }
        public ICollection<LoyaltyPointTransaction> Transactions { get; set; } = new List<LoyaltyPointTransaction>();
    }
}
