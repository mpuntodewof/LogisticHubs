namespace BlazorApp.Client.Models
{
    // ── Departments ──────────────────────────────────────────────────────────

    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateDepartmentRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    // ── Employees ────────────────────────────────────────────────────────────

    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string EmploymentStatus { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
    }

    public class CreateEmployeeRequest
    {
        public Guid UserId { get; set; }
        public Guid DepartmentId { get; set; }
        public string Position { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
    }

    // ── Attendance ───────────────────────────────────────────────────────────

    public class AttendanceDto
    {
        public Guid Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime? ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public decimal? WorkingHours { get; set; }
    }

    // ── Leave Requests ───────────────────────────────────────────────────────

    public class LeaveRequestDto
    {
        public Guid Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CreateLeaveRequestRequest
    {
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }
}
