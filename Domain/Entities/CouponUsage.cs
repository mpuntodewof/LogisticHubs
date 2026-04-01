using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class CouponUsage : BaseEntity, ITenantScoped
    {
        public Guid CouponCodeId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SalesOrderId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountApplied { get; set; }

        public DateTime UsedAt { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public CouponCode CouponCode { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public SalesOrder SalesOrder { get; set; } = null!;
    }
}
