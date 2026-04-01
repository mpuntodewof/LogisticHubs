using Domain.Interfaces;

namespace Domain.Entities
{
    public class NotificationPreference : BaseEntity, ITenantScoped
    {
        public Guid UserId { get; set; }
        public bool EnableEmail { get; set; } = true;
        public bool EnableSMS { get; set; }
        public bool EnablePush { get; set; } = true;
        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
