namespace Domain.Entities
{
    public class Permission
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;     // "shipments.create"
        public string Resource { get; set; } = string.Empty; // "shipments"
        public string Action { get; set; } = string.Empty;   // "create"

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}