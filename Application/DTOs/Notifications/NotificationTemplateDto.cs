using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Notifications
{
    public class NotificationTemplateDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string BodyTemplate { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateNotificationTemplateRequest
    {
        [Required, MaxLength(100)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public NotificationChannel Channel { get; set; }

        public string? Subject { get; set; }

        [Required]
        public string BodyTemplate { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    public class UpdateNotificationTemplateRequest
    {
        public string? Name { get; set; }
        public string? Subject { get; set; }
        public string? BodyTemplate { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
