using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class WebhookDelivery : BaseEntity, ITenantScoped
    {
        public Guid WebhookSubscriptionId { get; set; }
        [Required, MaxLength(200)]
        public string EntityType { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string EventType { get; set; } = string.Empty;
        public Guid? EntityId { get; set; }
        [Required, MaxLength(50)]
        public string Status { get; set; } = "Pending";
        public int? ResponseStatusCode { get; set; }
        [MaxLength(4000)]
        public string? ResponseBody { get; set; }
        [MaxLength(8000)]
        public string? PayloadJson { get; set; }
        public int RetryCount { get; set; }
        public DateTime? NextRetryAt { get; set; }
        [MaxLength(4000)]
        public string? ErrorMessage { get; set; }
        public int? DurationMs { get; set; }
        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public WebhookSubscription WebhookSubscription { get; set; } = null!;
    }
}
