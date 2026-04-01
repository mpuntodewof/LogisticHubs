using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Api
{
    public class WebhookSubscriptionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int MaxRetries { get; set; }
        public int TimeoutSeconds { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class WebhookSubscriptionDetailDto : WebhookSubscriptionDto
    {
        public string? HeadersJson { get; set; }
        public string? Secret { get; set; }
    }

    public class CreateWebhookSubscriptionRequest
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

        [MaxLength(4000)]
        public string? HeadersJson { get; set; }

        public int? MaxRetries { get; set; }
        public int? TimeoutSeconds { get; set; }
    }

    public class UpdateWebhookSubscriptionRequest
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(200)]
        public string? EntityType { get; set; }

        [MaxLength(50)]
        public string? EventType { get; set; }

        [MaxLength(1000)]
        public string? CallbackUrl { get; set; }

        [MaxLength(500)]
        public string? Secret { get; set; }

        [MaxLength(4000)]
        public string? HeadersJson { get; set; }

        public int? MaxRetries { get; set; }
        public int? TimeoutSeconds { get; set; }
        public bool? IsActive { get; set; }
    }
}
