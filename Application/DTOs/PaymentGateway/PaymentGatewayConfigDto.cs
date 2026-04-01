using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.PaymentGateway
{
    public class PaymentGatewayConfigDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string? MerchantId { get; set; }
        public bool IsActive { get; set; }
        public bool IsSandbox { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePaymentGatewayConfigRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public PaymentGatewayProvider Provider { get; set; }

        public string? MerchantId { get; set; }
        public string? ClientKey { get; set; }
        public string? ServerKey { get; set; }
        public string? WebhookSecret { get; set; }
        public string? BaseUrl { get; set; }
        public bool IsSandbox { get; set; } = true;
        public string? AdditionalConfig { get; set; }
    }

    public class UpdatePaymentGatewayConfigRequest
    {
        public string? Name { get; set; }
        public string? MerchantId { get; set; }
        public string? ClientKey { get; set; }
        public string? ServerKey { get; set; }
        public string? WebhookSecret { get; set; }
        public string? BaseUrl { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsSandbox { get; set; }
        public string? AdditionalConfig { get; set; }
    }
}
