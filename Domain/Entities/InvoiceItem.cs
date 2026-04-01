using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class InvoiceItem : BaseEntity, ITenantScoped
    {
        public Guid InvoiceId { get; set; }
        public Guid? SalesOrderItemId { get; set; }
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

        public Guid? TaxRateId { get; set; }

        [Column(TypeName = "decimal(5,4)")]
        public decimal TaxRateValue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public Invoice Invoice { get; set; } = null!;
        public SalesOrderItem? SalesOrderItem { get; set; }
        public ProductVariant ProductVariant { get; set; } = null!;
        public TaxRate? TaxRateEntity { get; set; }
    }
}
