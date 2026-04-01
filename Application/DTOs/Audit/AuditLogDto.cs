namespace Application.DTOs.Audit
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public Guid? EntityId { get; set; }
        public string? IpAddress { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class AuditLogDetailDto : AuditLogDto
    {
        public string? OldValuesJson { get; set; }
        public string? NewValuesJson { get; set; }
        public string? UserAgent { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}
