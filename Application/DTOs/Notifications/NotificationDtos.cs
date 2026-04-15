namespace Application.DTOs.Notifications
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty; // info, warning, critical
        public string? EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class NotificationSummary
    {
        public int TotalUnread { get; set; }
        public List<NotificationDto> Notifications { get; set; } = new();
    }
}
