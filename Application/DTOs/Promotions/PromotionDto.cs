using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Promotions
{
    public class PromotionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string PromotionType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? BuyQuantity { get; set; }
        public int? GetQuantity { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public int? MaxUsageCount { get; set; }
        public int MaxUsagePerCustomer { get; set; }
        public int CurrentUsageCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsStackable { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PromotionDetailDto : PromotionDto
    {
        public IList<PromotionRuleDto> Rules { get; set; } = new List<PromotionRuleDto>();
        public IList<PromotionProductDto> Products { get; set; } = new List<PromotionProductDto>();
    }

    public class CreatePromotionRequest
    {
        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Code { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public PromotionType PromotionType { get; set; }

        public DiscountType? DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? BuyQuantity { get; set; }
        public int? GetQuantity { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public int? MaxUsageCount { get; set; }
        public int MaxUsagePerCustomer { get; set; } = 1;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsStackable { get; set; }
        public int Priority { get; set; }
    }

    public class UpdatePromotionRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public int? MaxUsageCount { get; set; }
        public int? MaxUsagePerCustomer { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsStackable { get; set; }
        public int? Priority { get; set; }
        public bool? IsActive { get; set; }
    }
}
