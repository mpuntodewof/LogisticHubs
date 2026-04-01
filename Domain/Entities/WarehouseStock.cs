using Domain.Interfaces;

namespace Domain.Entities
{
    public class WarehouseStock : BaseEntity, ITenantScoped
    {
        public Guid WarehouseId { get; set; }
        public Guid ProductVariantId { get; set; }

        public int QuantityOnHand { get; set; }
        public int QuantityReserved { get; set; }
        public int QuantityAvailable => QuantityOnHand - QuantityReserved;

        public int? ReorderPoint { get; set; }
        public int? MaxStock { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
