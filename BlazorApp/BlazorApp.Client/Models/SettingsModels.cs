namespace BlazorApp.Client.Models
{
    // ── Tenant Settings ──────────────────────────────────────────────────────

    public class TenantSettingDto
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string? Value { get; set; }
        public string? Description { get; set; }
        public string? Group { get; set; }
        public string ValueType { get; set; } = string.Empty;
        public bool IsReadOnly { get; set; }
    }

    // NotificationDto is defined in ReportModels.cs

    // ── Reports ──────────────────────────────────────────────────────────────

    public class ReportDefinitionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty;
        public string? ScheduleFrequency { get; set; }
        public bool IsActive { get; set; }
    }

    // ── Audit Logs ───────────────────────────────────────────────────────────

    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public string? UserEmail { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // ── API Keys ─────────────────────────────────────────────────────────────

    public class ApiKeyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string KeyPrefix { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? LastUsedAt { get; set; }
    }

    // ── Webhook Subscriptions ────────────────────────────────────────────────

    public class WebhookSubscriptionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    // ── Banners ──────────────────────────────────────────────────────────────

    public class BannerDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CreateBannerRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    // ── Pages ────────────────────────────────────────────────────────────────

    public class PageDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CreatePageRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
    }

    // ── Storefront Config ────────────────────────────────────────────────────

    public class StorefrontConfigDto
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string? Value { get; set; }
    }
}
