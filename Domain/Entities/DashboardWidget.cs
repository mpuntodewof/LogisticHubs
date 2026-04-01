using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class DashboardWidget : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string WidgetType { get; set; } = string.Empty;
        [MaxLength(200)]
        public string? DataSourceKey { get; set; }
        [MaxLength(8000)]
        public string? ConfigJson { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; } = 1;
        public int Height { get; set; } = 1;
        public int SortOrder { get; set; }
        public bool IsVisible { get; set; } = true;
        public Guid? UserId { get; set; }
        public Guid TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public User? User { get; set; }
    }
}
