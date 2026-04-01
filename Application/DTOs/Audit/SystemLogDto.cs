namespace Application.DTOs.Audit
{
    public class SystemLogDto
    {
        public Guid Id { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Source { get; set; }
        public string? RequestPath { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class SystemLogDetailDto : SystemLogDto
    {
        public string? Exception { get; set; }
        public string? RequestMethod { get; set; }
        public string? AdditionalDataJson { get; set; }
    }
}
