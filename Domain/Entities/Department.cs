using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Department : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        public Guid? ParentDepartmentId { get; set; }
        public Guid? ManagerId { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public Department? ParentDepartment { get; set; }
        public ICollection<Department> ChildDepartments { get; set; } = new List<Department>();
        public Employee? Manager { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
