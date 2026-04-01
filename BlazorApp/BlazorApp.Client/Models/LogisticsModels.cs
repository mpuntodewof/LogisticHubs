namespace BlazorApp.Client.Models
{
    // ── Delivery Zones ───────────────────────────────────────────────────────

    public class DeliveryZoneDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Province { get; set; }
        public int EstimatedDeliveryDays { get; set; }
        public int MaxDeliveryDays { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateDeliveryZoneRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Province { get; set; }
        public int EstimatedDeliveryDays { get; set; }
        public int MaxDeliveryDays { get; set; }
    }

    // ── Delivery Rates ───────────────────────────────────────────────────────

    public class DeliveryRateDto
    {
        public Guid Id { get; set; }
        public string DeliveryZoneName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string RateType { get; set; } = string.Empty;
        public decimal FlatRateAmount { get; set; }
        public decimal PerKgRate { get; set; }
        public bool IsActive { get; set; }
    }

    // ── Branches ─────────────────────────────────────────────────────────────

    public class BranchDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? WarehouseName { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateBranchRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Province { get; set; }
        public Guid? WarehouseId { get; set; }
    }
}
