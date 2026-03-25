using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Roles
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystem { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new();
    }

    public class PermissionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
    }

    public class CreateRoleRequest
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class UpdateRoleRequest
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class UpdateRolePermissionsRequest
    {
        [Required]
        public List<Guid> PermissionIds { get; set; } = new();
    }
}
