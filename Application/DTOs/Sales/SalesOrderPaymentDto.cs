using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Sales
{
    public class SalesOrderPaymentDto
    {
        public Guid Id { get; set; }
        public Guid SalesOrderId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateSalesOrderPaymentRequest
    {
        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public decimal Amount { get; set; }

        [StringLength(100)]
        public string? ReferenceNumber { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
