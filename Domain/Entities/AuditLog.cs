using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class AuditLog : BaseEntity, ITenantScoped
    {
        public Guid? UserId { get; set; }
        [MaxLength(255)]
        public string? UserEmail { get; set; }
        [Required, MaxLength(50)]
        public string Action { get; set; } = string.Empty;
        [Required, MaxLength(200)]
        public string EntityType { get; set; } = string.Empty;
        public Guid? EntityId { get; set; }
        [MaxLength(8000)]
        public string? OldValuesJson { get; set; }
        [MaxLength(8000)]
        public string? NewValuesJson { get; set; }
        [MaxLength(100)]
        public string? IpAddress { get; set; }
        [MaxLength(500)]
        public string? UserAgent { get; set; }
        [MaxLength(500)]
        public string? AdditionalInfo { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
    }
}
