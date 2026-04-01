using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class PaymentGatewayConfig : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Provider { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? MerchantId { get; set; }

        [MaxLength(500)]
        public string? ClientKey { get; set; }

        [MaxLength(500)]
        public string? ServerKey { get; set; }

        [MaxLength(500)]
        public string? WebhookSecret { get; set; }

        [MaxLength(500)]
        public string? BaseUrl { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsSandbox { get; set; } = true;

        [MaxLength(4000)]
        public string? AdditionalConfig { get; set; }

        public Guid TenantId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
    }
}
