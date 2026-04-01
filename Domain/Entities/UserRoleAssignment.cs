using Domain.Interfaces;

namespace Domain.Entities
{
    public class UserRoleAssignment : ITenantScoped
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public Guid? AssignedBy { get; set; }

        public Guid TenantId { get; set; }

        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}