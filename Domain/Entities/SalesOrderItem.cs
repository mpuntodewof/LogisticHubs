using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class SalesOrderItem : BaseEntity, ITenantScoped
    {
        public Guid SalesOrderId { get; set; }
        public Guid ProductVariantId { get; set; }

        [Required]
        [MaxLength(500)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string VariantName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Sku { get; set; } = string.Empty;

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public SalesOrder SalesOrder { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
