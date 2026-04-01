using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class ApiKey : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required, MaxLength(500)]
        public string KeyHash { get; set; } = string.Empty;
        [MaxLength(20)]
        public string KeyPrefix { get; set; } = string.Empty;
        [MaxLength(4000)]
        public string? PermissionsJson { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public int RateLimitPerMinute { get; set; } = 60;
        public int RateLimitPerDay { get; set; } = 10000;
        public bool IsActive { get; set; } = true;
        public DateTime? LastUsedAt { get; set; }
        [MaxLength(100)]
        public string? LastUsedIp { get; set; }
        public Guid TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public Tenant Tenant { get; set; } = null!;
    }
}
