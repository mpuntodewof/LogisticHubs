using Domain.Interfaces;

namespace Domain.Entities
{
    public class Permission : BaseEntity, ITenantScoped
    {
        public string Name { get; set; } = string.Empty;     // "shipments.create"
        public string Resource { get; set; } = string.Empty; // "shipments"
        public string Action { get; set; } = string.Empty;   // "create"

        public Guid TenantId { get; set; }

        // Navigation properties
        public Tenant Tenant { get; set; } = null!;
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}