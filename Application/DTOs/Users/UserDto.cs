namespace Application.DTOs.Users
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<string> Roles { get; set; } = [];
    }

    public class UpdateUserRequest
    {
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
    }

    public class AssignRoleRequest
    {
        public Guid RoleId { get; set; }
    }
}