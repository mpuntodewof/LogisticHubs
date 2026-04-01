using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Ecommerce
{
    public class ProductReviewDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Title { get; set; }
        public string? Comment { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsVerifiedPurchase { get; set; }
        public string? AdminResponse { get; set; }
        public DateTime? AdminRespondedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateProductReviewRequest
    {
        [Required]
        public Guid ProductId { get; set; }

        public Guid? SalesOrderId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Title { get; set; }
        public string? Comment { get; set; }
    }

    public class ModerateReviewRequest
    {
        [Required]
        public ReviewStatus Status { get; set; }

        public string? AdminResponse { get; set; }
    }

    public class ProductRatingSummaryDto
    {
        public Guid ProductId { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
