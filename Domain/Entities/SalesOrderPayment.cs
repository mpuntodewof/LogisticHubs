using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class SalesOrderPayment : BaseEntity, ITenantScoped
    {
        public Guid SalesOrderId { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string PaymentStatus { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        public DateTime? PaidAt { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public SalesOrder SalesOrder { get; set; } = null!;
    }
}
