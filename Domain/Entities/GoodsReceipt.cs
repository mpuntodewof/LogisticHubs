using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class GoodsReceipt : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required, MaxLength(50)]
        public string ReceiptNumber { get; set; } = string.Empty;
        public Guid PurchaseOrderId { get; set; }
        public Guid WarehouseId { get; set; }
        [Required, MaxLength(50)]
        public string Status { get; set; } = "Draft";
        public DateTime ReceivedDate { get; set; }
        public Guid? ReceivedBy { get; set; }
        [MaxLength(2000)]
        public string? Notes { get; set; }
        public Guid TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public ICollection<GoodsReceiptItem> Items { get; set; } = new List<GoodsReceiptItem>();
    }
}
