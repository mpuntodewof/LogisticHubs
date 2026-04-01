using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Inventory
{
    public class StockMovementDto
    {
        public Guid Id { get; set; }
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public Guid ProductVariantId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ProductVariantName { get; set; } = string.Empty;
        public string MovementType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int QuantityBefore { get; set; }
        public int QuantityAfter { get; set; }
        public string? ReferenceDocumentType { get; set; }
        public Guid? ReferenceDocumentId { get; set; }
        public string? ReferenceDocumentNumber { get; set; }
        public Guid? SourceWarehouseId { get; set; }
        public Guid? DestinationWarehouseId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
    }

    public class CreateStockMovementRequest
    {
        public Guid WarehouseId { get; set; }
        public Guid ProductVariantId { get; set; }
        public StockMovementType MovementType { get; set; }
        public StockMovementReason Reason { get; set; }
        public int Quantity { get; set; }

        [StringLength(100)]
        public string? ReferenceDocumentType { get; set; }

        public Guid? ReferenceDocumentId { get; set; }

        [StringLength(100)]
        public string? ReferenceDocumentNumber { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class CreateStockTransferRequest
    {
        public Guid SourceWarehouseId { get; set; }
        public Guid DestinationWarehouseId { get; set; }
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }

        [StringLength(100)]
        public string? ReferenceDocumentNumber { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}
