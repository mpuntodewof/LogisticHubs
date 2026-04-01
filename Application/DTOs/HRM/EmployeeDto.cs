using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.HRM
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string Position { get; set; } = string.Empty;
        public string EmploymentStatus { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public decimal BaseSalary { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EmployeeDetailDto : EmployeeDto
    {
        public DateTime? TerminationDate { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateEmployeeRequest
    {
        [Required]
        public Guid UserId { get; set; }

        public Guid? DepartmentId { get; set; }

        [Required]
        public string Position { get; set; } = string.Empty;

        [Required]
        public DateTime HireDate { get; set; }

        [Required]
        public decimal BaseSalary { get; set; }

        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateEmployeeRequest
    {
        public Guid? DepartmentId { get; set; }
        public string? Position { get; set; }
        public Domain.Enums.EmploymentStatus? EmploymentStatus { get; set; }
        public decimal? BaseSalary { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
    }
}
