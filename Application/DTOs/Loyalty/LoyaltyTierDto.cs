using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Loyalty
{
    public class LoyaltyTierDto
    {
        public Guid Id { get; set; }
        public Guid LoyaltyProgramId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MinPointsThreshold { get; set; }
        public decimal PointsMultiplier { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateLoyaltyTierRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int MinPointsThreshold { get; set; }

        public decimal? PointsMultiplier { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
    }

    public class UpdateLoyaltyTierRequest
    {
        public string? Name { get; set; }
        public int? MinPointsThreshold { get; set; }
        public decimal? PointsMultiplier { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string? Description { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }
    }
}
