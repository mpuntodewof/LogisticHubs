using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class PaymentTransaction : BaseEntity, ITenantScoped
    {
        [Required]
        [MaxLength(100)]
        public string TransactionNumber { get; set; } = string.Empty;

        public Guid SalesOrderPaymentId { get; set; }
        public Guid PaymentGatewayConfigId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Provider { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? ExternalTransactionId { get; set; }

        [MaxLength(200)]
        public string? ExternalReferenceId { get; set; }

        [MaxLength(1000)]
        public string? PaymentUrl { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "IDR";

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string? GatewayResponse { get; set; }

        [MaxLength(1000)]
        public string? FailureReason { get; set; }

        public DateTime? PaidAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public DateTime? RefundedAt { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public SalesOrderPayment SalesOrderPayment { get; set; } = null!;
        public PaymentGatewayConfig PaymentGatewayConfig { get; set; } = null!;
    }
}
