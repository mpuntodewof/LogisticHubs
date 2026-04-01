namespace Application.DTOs.Reporting
{
    public class ReportExecutionDto
    {
        public Guid Id { get; set; }
        public Guid ReportDefinitionId { get; set; }
        public string ReportName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? DurationMs { get; set; }
        public string? OutputFileUrl { get; set; }
        public string? OutputFormat { get; set; }
        public Guid? TriggeredBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ReportExecutionDetailDto : ReportExecutionDto
    {
        public string? ErrorMessage { get; set; }
        public string? ParametersUsedJson { get; set; }
    }

    public class TriggerReportExecutionRequest
    {
        public string OutputFormat { get; set; } = "PDF";
    }
}
