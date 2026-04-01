using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class ShoppingCartItem : BaseEntity, ITenantScoped
    {
        public Guid ShoppingCartId { get; set; }
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public ShoppingCart ShoppingCart { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
