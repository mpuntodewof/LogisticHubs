using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Logistics
{
    public class DeliveryZoneDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CoverageAreas { get; set; } = "[]";
        public string? Province { get; set; }
        public int EstimatedDeliveryDays { get; set; }
        public int MaxDeliveryDays { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateDeliveryZoneRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string CoverageAreas { get; set; } = "[]";

        public string? Province { get; set; }

        public int EstimatedDeliveryDays { get; set; }

        public int MaxDeliveryDays { get; set; }
    }

    public class UpdateDeliveryZoneRequest
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? CoverageAreas { get; set; }
        public string? Province { get; set; }
        public int? EstimatedDeliveryDays { get; set; }
        public int? MaxDeliveryDays { get; set; }
        public bool? IsActive { get; set; }
    }
}
