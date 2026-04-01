using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Wishlist : BaseEntity, ITenantScoped
    {
        public Guid CustomerId { get; set; }

        [MaxLength(200)]
        public string Name { get; set; } = "My Wishlist";

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
    }
}
