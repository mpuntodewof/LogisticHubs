using Domain.Interfaces;

namespace Domain.Entities
{
    public class PromotionProduct : BaseEntity, ITenantScoped
    {
        public Guid PromotionId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public bool IsGetItem { get; set; }

        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public Promotion Promotion { get; set; } = null!;
        public Product? Product { get; set; }
        public ProductVariant? ProductVariant { get; set; }
    }
}
