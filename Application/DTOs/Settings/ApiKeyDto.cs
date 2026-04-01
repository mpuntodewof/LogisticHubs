using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Settings
{
    public class ApiKeyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string KeyPrefix { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
        public int RateLimitPerMinute { get; set; }
        public int RateLimitPerDay { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ApiKeyDetailDto : ApiKeyDto
    {
        public string? PermissionsJson { get; set; }
        public string? LastUsedIp { get; set; }
    }

    public class CreateApiKeyRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? PermissionsJson { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int? RateLimitPerMinute { get; set; }
        public int? RateLimitPerDay { get; set; }
    }

    public class CreateApiKeyResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string KeyPrefix { get; set; } = string.Empty;
        public string PlainTextKey { get; set; } = string.Empty;
    }

    public class UpdateApiKeyRequest
    {
        public string? Name { get; set; }
        public string? PermissionsJson { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int? RateLimitPerMinute { get; set; }
        public int? RateLimitPerDay { get; set; }
        public bool? IsActive { get; set; }
    }
}
