using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Logistics
{
    public class DeliveryRateDto
    {
        public Guid Id { get; set; }
        public Guid DeliveryZoneId { get; set; }
        public string DeliveryZoneName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string RateType { get; set; } = string.Empty;
        public decimal FlatRateAmount { get; set; }
        public decimal PerKgRate { get; set; }
        public decimal MinWeight { get; set; }
        public decimal MaxWeight { get; set; }
        public decimal WeightRangeRate { get; set; }
        public decimal? MinOrderAmountForFree { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateDeliveryRateRequest
    {
        [Required]
        public Guid DeliveryZoneId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DeliveryRateType RateType { get; set; }

        public decimal FlatRateAmount { get; set; }
        public decimal PerKgRate { get; set; }
        public decimal MinWeight { get; set; }
        public decimal MaxWeight { get; set; }
        public decimal WeightRangeRate { get; set; }
        public decimal? MinOrderAmountForFree { get; set; }
    }

    public class UpdateDeliveryRateRequest
    {
        public Guid? DeliveryZoneId { get; set; }
        public string? Name { get; set; }
        public DeliveryRateType? RateType { get; set; }
        public decimal? FlatRateAmount { get; set; }
        public decimal? PerKgRate { get; set; }
        public decimal? MinWeight { get; set; }
        public decimal? MaxWeight { get; set; }
        public decimal? WeightRangeRate { get; set; }
        public decimal? MinOrderAmountForFree { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CalculateShippingRequest
    {
        [Required]
        public Guid DeliveryZoneId { get; set; }

        [Required]
        public decimal TotalWeight { get; set; }

        public decimal OrderAmount { get; set; }
    }

    public class CalculateShippingResponse
    {
        public Guid DeliveryZoneId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public decimal ShippingCost { get; set; }
        public int EstimatedDeliveryDays { get; set; }
        public int MaxDeliveryDays { get; set; }
        public bool IsFreeShipping { get; set; }
    }
}
