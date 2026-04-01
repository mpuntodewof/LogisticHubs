using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Notification : BaseEntity, ITenantScoped
    {
        public Guid UserId { get; set; }
        [Required, MaxLength(500)]
        public string Title { get; set; } = string.Empty;
        [Required, MaxLength(4000)]
        public string Message { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string Channel { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string Status { get; set; } = "Unread";
        [MaxLength(100)]
        public string? SourceEntityType { get; set; }
        public Guid? SourceEntityId { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? SentAt { get; set; }
        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
