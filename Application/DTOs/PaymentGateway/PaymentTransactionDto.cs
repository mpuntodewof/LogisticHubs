using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PaymentGateway
{
    public class PaymentTransactionDto
    {
        public Guid Id { get; set; }
        public string TransactionNumber { get; set; } = string.Empty;
        public Guid SalesOrderPaymentId { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string? ExternalTransactionId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "IDR";
        public string Status { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentTransactionDetailDto : PaymentTransactionDto
    {
        public string? ExternalReferenceId { get; set; }
        public string? PaymentUrl { get; set; }
        public string? GatewayResponse { get; set; }
        public string? FailureReason { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public DateTime? RefundedAt { get; set; }
    }

    public class InitiatePaymentRequest
    {
        [Required]
        public Guid SalesOrderPaymentId { get; set; }

        [Required]
        public Guid PaymentGatewayConfigId { get; set; }
    }
}
