using Domain.Interfaces;

namespace Domain.Entities
{
    public class RolePermission : ITenantScoped
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        public Guid TenantId { get; set; }

        public Role Role { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}