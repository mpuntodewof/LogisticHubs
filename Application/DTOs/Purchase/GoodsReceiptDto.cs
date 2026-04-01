using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Purchase
{
    public class GoodsReceiptDto
    {
        public Guid Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public Guid PurchaseOrderId { get; set; }
        public string? PoNumber { get; set; }
        public Guid WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ReceivedDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GoodsReceiptDetailDto : GoodsReceiptDto
    {
        public IList<GoodsReceiptItemDto> Items { get; set; } = new List<GoodsReceiptItemDto>();
        public string? Notes { get; set; }
    }

    public class CreateGoodsReceiptRequest
    {
        [Required]
        public Guid PurchaseOrderId { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }

        [Required]
        public List<CreateGoodsReceiptItemRequest> Items { get; set; } = new();
    }

    public class CreateGoodsReceiptItemRequest
    {
        [Required]
        public Guid PurchaseOrderItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int QuantityReceived { get; set; }

        public int? QuantityRejected { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class GoodsReceiptItemDto
    {
        public Guid Id { get; set; }
        public Guid GoodsReceiptId { get; set; }
        public Guid PurchaseOrderItemId { get; set; }
        public Guid ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int QuantityReceived { get; set; }
        public int QuantityRejected { get; set; }
        public string? Notes { get; set; }
    }
}
