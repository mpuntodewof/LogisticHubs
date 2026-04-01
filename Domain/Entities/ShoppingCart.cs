using Domain.Interfaces;

namespace Domain.Entities
{
    public class ShoppingCart : BaseEntity, ITenantScoped
    {
        public Guid? CustomerId { get; set; }
        public Guid? GuestId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? ExpiresAt { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public Customer? Customer { get; set; }
        public ICollection<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
    }
}
