using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.HRM
{
    public class AttendanceDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public DateOnly Date { get; set; }
        public DateTime ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public decimal? WorkingHours { get; set; }
        public string? Notes { get; set; }
    }

    public class ClockInRequest
    {
        [Required]
        public Guid EmployeeId { get; set; }

        public string? Notes { get; set; }
    }
}
