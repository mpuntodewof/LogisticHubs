using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class PurchaseOrder : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required, MaxLength(50)]
        public string PoNumber { get; set; } = string.Empty;
        public Guid SupplierId { get; set; }
        public Guid WarehouseId { get; set; }
        public Guid? BranchId { get; set; }
        [Required, MaxLength(50)]
        public string Status { get; set; } = "Draft";
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal GrandTotal { get; set; }
        [MaxLength(2000)]
        public string? Notes { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        [MaxLength(1000)]
        public string? CancellationReason { get; set; }
        public Guid TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public Supplier Supplier { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
        public Branch? Branch { get; set; }
        public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
        public ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();
    }
}
