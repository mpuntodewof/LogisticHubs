using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class ReportExecution : BaseEntity, ITenantScoped
    {
        public Guid ReportDefinitionId { get; set; }
        [Required, MaxLength(50)]
        public string Status { get; set; } = "Queued";
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? DurationMs { get; set; }
        [MaxLength(1000)]
        public string? OutputFileUrl { get; set; }
        [MaxLength(50)]
        public string? OutputFormat { get; set; }
        [MaxLength(4000)]
        public string? ErrorMessage { get; set; }
        [MaxLength(8000)]
        public string? ParametersUsedJson { get; set; }
        public Guid? TriggeredBy { get; set; }
        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public ReportDefinition ReportDefinition { get; set; } = null!;
    }
}
