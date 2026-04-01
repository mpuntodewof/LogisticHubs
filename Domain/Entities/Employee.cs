using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Employee : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required, MaxLength(50)]
        public string EmployeeCode { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Guid? DepartmentId { get; set; }
        [Required, MaxLength(255)]
        public string Position { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string EmploymentStatus { get; set; } = "Active";
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseSalary { get; set; }
        [MaxLength(255)]
        public string? BankName { get; set; }
        [MaxLength(100)]
        public string? BankAccountNumber { get; set; }
        [MaxLength(255)]
        public string? BankAccountName { get; set; }
        [MaxLength(50)]
        public string? Phone { get; set; }
        [MaxLength(1000)]
        public string? Address { get; set; }
        [MaxLength(2000)]
        public string? Notes { get; set; }
        public Guid TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public User User { get; set; } = null!;
        public Department? Department { get; set; }
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    }
}
