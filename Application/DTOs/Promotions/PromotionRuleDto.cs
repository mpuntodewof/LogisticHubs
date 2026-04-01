using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Promotions
{
    public class PromotionRuleDto
    {
        public Guid Id { get; set; }
        public Guid PromotionId { get; set; }
        public string RuleType { get; set; } = string.Empty;
        public int? MinQuantity { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public Guid? CustomerGroupId { get; set; }
        public string? CustomerGroupName { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }

    public class CreatePromotionRuleRequest
    {
        [Required]
        public PromotionRuleType RuleType { get; set; }

        public int? MinQuantity { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public Guid? CustomerGroupId { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
