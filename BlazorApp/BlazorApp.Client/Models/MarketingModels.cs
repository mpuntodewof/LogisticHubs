namespace BlazorApp.Client.Models
{
    // ── Promotions ───────────────────────────────────────────────────────────

    public class PromotionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string PromotionType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreatePromotionRequest
    {
        public string Name { get; set; } = string.Empty;
        public string PromotionType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    // ── Coupon Codes ─────────────────────────────────────────────────────────

    public class CouponCodeDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CurrentUsageCount { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCouponCodeRequest
    {
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? MaxUsageCount { get; set; }
    }

    // ── Loyalty Programs ─────────────────────────────────────────────────────

    public class LoyaltyProgramDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal PointsPerIdrSpent { get; set; }
        public decimal RedemptionRateIdr { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    // ── Loyalty Memberships ──────────────────────────────────────────────────

    public class LoyaltyMembershipDto
    {
        public Guid Id { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CurrentTierName { get; set; } = string.Empty;
        public int AvailablePoints { get; set; }
        public int LifetimePoints { get; set; }
    }
}
