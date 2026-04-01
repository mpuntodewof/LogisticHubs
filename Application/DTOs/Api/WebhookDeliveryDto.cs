namespace Application.DTOs.Api
{
    public class WebhookDeliveryDto
    {
        public Guid Id { get; set; }
        public Guid WebhookSubscriptionId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public Guid? EntityId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? ResponseStatusCode { get; set; }
        public int RetryCount { get; set; }
        public int? DurationMs { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class WebhookDeliveryDetailDto : WebhookDeliveryDto
    {
        public string? ResponseBody { get; set; }
        public string? PayloadJson { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? NextRetryAt { get; set; }
    }
}
