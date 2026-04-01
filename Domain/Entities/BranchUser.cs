using Domain.Interfaces;

namespace Domain.Entities
{
    public class BranchUser : ITenantScoped
    {
        public Guid BranchId { get; set; }
        public Guid UserId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public Guid? AssignedBy { get; set; }
        public bool IsActive { get; set; } = true;

        public Guid TenantId { get; set; }

        // Navigation
        public Branch Branch { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
