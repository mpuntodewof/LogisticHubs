using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Reporting
{
    public class ReportDefinitionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string ScheduleFrequency { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ReportDefinitionDetailDto : ReportDefinitionDto
    {
        public string? ParametersJson { get; set; }
        public string? ScheduleTime { get; set; }
        public int? ScheduleDayOfWeek { get; set; }
        public int? ScheduleDayOfMonth { get; set; }
    }

    public class CreateReportDefinitionRequest
    {
        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public ReportType ReportType { get; set; }

        public string? ParametersJson { get; set; }
        public ReportScheduleFrequency ScheduleFrequency { get; set; } = ReportScheduleFrequency.None;

        [StringLength(10)]
        public string? ScheduleTime { get; set; }

        public int? ScheduleDayOfWeek { get; set; }
        public int? ScheduleDayOfMonth { get; set; }
    }

    public class UpdateReportDefinitionRequest
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public ReportType? ReportType { get; set; }
        public string? ParametersJson { get; set; }
        public ReportScheduleFrequency? ScheduleFrequency { get; set; }

        [StringLength(10)]
        public string? ScheduleTime { get; set; }

        public int? ScheduleDayOfWeek { get; set; }
        public int? ScheduleDayOfMonth { get; set; }
        public bool? IsActive { get; set; }
    }
}
