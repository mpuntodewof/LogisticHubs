using Domain.Interfaces;

namespace Domain.Entities
{
    public class WishlistItem : BaseEntity, ITenantScoped
    {
        public Guid WishlistId { get; set; }
        public Guid ProductVariantId { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public Wishlist Wishlist { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
