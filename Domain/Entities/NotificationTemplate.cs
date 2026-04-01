using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class NotificationTemplate : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required, MaxLength(100)]
        public string Code { get; set; } = string.Empty;
        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string Channel { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Subject { get; set; }
        [Required, MaxLength(4000)]
        public string BodyTemplate { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public Tenant Tenant { get; set; } = null!;
    }
}
