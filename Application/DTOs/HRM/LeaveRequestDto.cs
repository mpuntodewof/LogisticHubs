using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.HRM
{
    public class LeaveRequestDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateLeaveRequestRequest
    {
        [Required]
        public Guid EmployeeId { get; set; }

        [Required]
        public Domain.Enums.LeaveType LeaveType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string? Reason { get; set; }
    }

    public class RejectLeaveRequestRequest
    {
        [Required]
        public string RejectionReason { get; set; } = string.Empty;
    }
}
