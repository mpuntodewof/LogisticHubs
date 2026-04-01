using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Ecommerce
{
    public class CouponCodeDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? MaxUsageCount { get; set; }
        public int MaxUsagePerCustomer { get; set; }
        public int CurrentUsageCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCouponCodeRequest
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DiscountType DiscountType { get; set; }

        [Required]
        public decimal DiscountValue { get; set; }

        public decimal? MinimumOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int? MaxUsageCount { get; set; }
        public int MaxUsagePerCustomer { get; set; } = 1;
    }

    public class UpdateCouponCodeRequest
    {
        public string? Description { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxUsageCount { get; set; }
        public int? MaxUsagePerCustomer { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ValidateCouponRequest
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public decimal OrderAmount { get; set; }
    }

    public class ValidateCouponResponse
    {
        public bool IsValid { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class CouponUsageDto
    {
        public Guid Id { get; set; }
        public Guid CouponCodeId { get; set; }
        public string CouponCode { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid SalesOrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal DiscountApplied { get; set; }
        public DateTime UsedAt { get; set; }
    }
}
