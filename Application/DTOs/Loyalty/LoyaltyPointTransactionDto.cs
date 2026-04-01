using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Loyalty
{
    public class LoyaltyPointTransactionDto
    {
        public Guid Id { get; set; }
        public Guid LoyaltyMembershipId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public int Points { get; set; }
        public int BalanceBefore { get; set; }
        public int BalanceAfter { get; set; }
        public string? Description { get; set; }
        public string? ReferenceDocumentType { get; set; }
        public string? ReferenceDocumentNumber { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class AdjustPointsRequest
    {
        [Required]
        public int Points { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
    }

    public class RedeemPointsRequest
    {
        [Required]
        public int Points { get; set; }

        [Required]
        public Guid SalesOrderId { get; set; }
    }
}
