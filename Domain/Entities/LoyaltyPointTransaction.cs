using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class LoyaltyPointTransaction : BaseEntity, ITenantScoped
    {
        public Guid LoyaltyMembershipId { get; set; }

        [Required, MaxLength(50)]
        public string TransactionType { get; set; } = string.Empty;

        public int Points { get; set; }
        public int BalanceBefore { get; set; }
        public int BalanceAfter { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? ReferenceDocumentType { get; set; }

        public Guid? ReferenceDocumentId { get; set; }

        [MaxLength(100)]
        public string? ReferenceDocumentNumber { get; set; }

        public DateTime? ExpiresAt { get; set; }
        public DateTime TransactionDate { get; set; }

        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public LoyaltyMembership LoyaltyMembership { get; set; } = null!;
    }
}
