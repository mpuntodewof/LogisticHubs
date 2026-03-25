namespace BlazorApp.Client.Models
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class UpdateUserRequest
    {
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CreateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AssignRoleRequest
    {
        public Guid RoleId { get; set; }
    }
}
