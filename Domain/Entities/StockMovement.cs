using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class StockMovement : BaseEntity, ITenantScoped
    {
        public Guid WarehouseId { get; set; }
        public Guid ProductVariantId { get; set; }

        [Required]
        [MaxLength(50)]
        public string MovementType { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Reason { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public int QuantityBefore { get; set; }
        public int QuantityAfter { get; set; }

        [MaxLength(100)]
        public string? ReferenceDocumentType { get; set; }

        public Guid? ReferenceDocumentId { get; set; }

        [MaxLength(100)]
        public string? ReferenceDocumentNumber { get; set; }

        public Guid? SourceWarehouseId { get; set; }
        public Guid? DestinationWarehouseId { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
        public Warehouse? SourceWarehouse { get; set; }
        public Warehouse? DestinationWarehouse { get; set; }
    }
}
