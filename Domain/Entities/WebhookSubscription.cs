using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class WebhookSubscription : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required, MaxLength(200)]
        public string EntityType { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string EventType { get; set; } = string.Empty;
        [Required, MaxLength(1000)]
        public string CallbackUrl { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Secret { get; set; }
        public bool IsActive { get; set; } = true;
        [MaxLength(4000)]
        public string? HeadersJson { get; set; }
        public int MaxRetries { get; set; } = 3;
        public int TimeoutSeconds { get; set; } = 30;
        public Guid TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public ICollection<WebhookDelivery> Deliveries { get; set; } = new List<WebhookDelivery>();
    }
}
