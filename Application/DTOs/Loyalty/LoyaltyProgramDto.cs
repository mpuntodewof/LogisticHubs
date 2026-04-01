using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Loyalty
{
    public class LoyaltyProgramDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal PointsPerIdrSpent { get; set; }
        public decimal RedemptionRateIdr { get; set; }
        public int MinRedemptionPoints { get; set; }
        public int? PointExpiryDays { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class LoyaltyProgramDetailDto : LoyaltyProgramDto
    {
        public List<LoyaltyTierDto> Tiers { get; set; } = new();
    }

    public class CreateLoyaltyProgramRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public decimal PointsPerIdrSpent { get; set; }

        [Required]
        public decimal RedemptionRateIdr { get; set; }

        public int? MinRedemptionPoints { get; set; }
        public int? PointExpiryDays { get; set; }
    }

    public class UpdateLoyaltyProgramRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? PointsPerIdrSpent { get; set; }
        public decimal? RedemptionRateIdr { get; set; }
        public int? MinRedemptionPoints { get; set; }
        public int? PointExpiryDays { get; set; }
        public bool? IsActive { get; set; }
        public LoyaltyProgramStatus? Status { get; set; }
    }
}
