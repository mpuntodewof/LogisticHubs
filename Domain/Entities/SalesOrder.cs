using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class SalesOrder : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required]
        [MaxLength(50)]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string OrderType { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public Guid? CustomerId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? ShippingAddressId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GrandTotal { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        [MaxLength(1000)]
        public string? CancellationReason { get; set; }

        public Guid? ShipmentId { get; set; }

        public Guid TenantId { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public Customer? Customer { get; set; }
        public Branch? Branch { get; set; }
        public CustomerAddress? ShippingAddress { get; set; }
        public Shipment? Shipment { get; set; }
        public ICollection<SalesOrderItem> Items { get; set; } = new List<SalesOrderItem>();
        public ICollection<SalesOrderPayment> Payments { get; set; } = new List<SalesOrderPayment>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();
        public ICollection<PromotionUsage> PromotionUsages { get; set; } = new List<PromotionUsage>();
    }
}
