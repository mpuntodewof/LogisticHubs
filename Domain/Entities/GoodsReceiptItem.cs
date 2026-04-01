using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class GoodsReceiptItem : BaseEntity, ITenantScoped
    {
        public Guid GoodsReceiptId { get; set; }
        public Guid PurchaseOrderItemId { get; set; }
        public Guid ProductVariantId { get; set; }
        public int QuantityReceived { get; set; }
        public int QuantityRejected { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public GoodsReceipt GoodsReceipt { get; set; } = null!;
        public PurchaseOrderItem PurchaseOrderItem { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
