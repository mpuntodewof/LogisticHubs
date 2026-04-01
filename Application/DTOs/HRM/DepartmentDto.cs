using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.HRM
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ParentDepartmentId { get; set; }
        public string? ParentDepartmentName { get; set; }
        public Guid? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateDepartmentRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }
        public Guid? ParentDepartmentId { get; set; }
        public Guid? ManagerId { get; set; }
    }

    public class UpdateDepartmentRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentDepartmentId { get; set; }
        public Guid? ManagerId { get; set; }
        public bool? IsActive { get; set; }
    }
}
