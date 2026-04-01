using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class PromotionUsage : BaseEntity, ITenantScoped
    {
        public Guid PromotionId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SalesOrderId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountApplied { get; set; }

        public DateTime UsedAt { get; set; }

        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public Promotion Promotion { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public SalesOrder SalesOrder { get; set; } = null!;
    }
}
