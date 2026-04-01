using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Attendance : BaseEntity, ITenantScoped
    {
        public Guid EmployeeId { get; set; }
        public DateTime ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public DateOnly Date { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal? WorkingHours { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
    }
}
