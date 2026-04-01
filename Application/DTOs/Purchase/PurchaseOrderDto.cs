using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Purchase
{
    public class PurchaseOrderDto
    {
        public Guid Id { get; set; }
        public string PoNumber { get; set; } = string.Empty;
        public Guid SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public Guid WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public Guid? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PurchaseOrderDetailDto : PurchaseOrderDto
    {
        public IList<PurchaseOrderItemDto> Items { get; set; } = new List<PurchaseOrderItemDto>();
        public string? Notes { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }

    public class CreatePurchaseOrderRequest
    {
        [Required]
        public Guid SupplierId { get; set; }

        [Required]
        public Guid WarehouseId { get; set; }

        public Guid? BranchId { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }

        [Required]
        public List<CreatePurchaseOrderItemRequest> Items { get; set; } = new();
    }

    public class UpdatePurchaseOrderRequest
    {
        public DateTime? ExpectedDeliveryDate { get; set; }
        public decimal? DiscountAmount { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }
    }

    public class CreatePurchaseOrderItemRequest
    {
        [Required]
        public Guid ProductVariantId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitCost { get; set; }

        public decimal? DiscountAmount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class PurchaseOrderItemDto
    {
        public Guid Id { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public Guid ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineTotal { get; set; }
        public int ReceivedQuantity { get; set; }
    }
}
