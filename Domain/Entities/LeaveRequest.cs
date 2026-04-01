using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class LeaveRequest : BaseEntity, ITenantScoped
    {
        public Guid EmployeeId { get; set; }
        [Required, MaxLength(50)]
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        [Required, MaxLength(50)]
        public string Status { get; set; } = "Pending";
        [MaxLength(2000)]
        public string? Reason { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        [MaxLength(1000)]
        public string? RejectionReason { get; set; }
        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
    }
}
