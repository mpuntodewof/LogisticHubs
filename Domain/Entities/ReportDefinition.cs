using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class ReportDefinition : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required, MaxLength(50)]
        public string ReportType { get; set; } = string.Empty;
        [MaxLength(8000)]
        public string? ParametersJson { get; set; }
        [Required, MaxLength(50)]
        public string ScheduleFrequency { get; set; } = "None";
        [MaxLength(10)]
        public string? ScheduleTime { get; set; }
        public int? ScheduleDayOfWeek { get; set; }
        public int? ScheduleDayOfMonth { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public ICollection<ReportExecution> Executions { get; set; } = new List<ReportExecution>();
    }
}
