namespace BlazorApp.Client.Models
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
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateRoleRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateRolePermissionsRequest
    {
        public List<Guid> PermissionIds { get; set; } = new();
    }
}
