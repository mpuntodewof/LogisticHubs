using Application.DTOs.Common;
using Application.DTOs.Logistics;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Logistics
{
    public class DeliveryRateUseCase
    {
        private readonly IDeliveryRateRepository _deliveryRateRepository;
        private readonly IDeliveryZoneRepository _deliveryZoneRepository;

        public DeliveryRateUseCase(IDeliveryRateRepository deliveryRateRepository, IDeliveryZoneRepository deliveryZoneRepository)
        {
            _deliveryRateRepository = deliveryRateRepository;
            _deliveryZoneRepository = deliveryZoneRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<DeliveryRateDto>> GetPagedAsync(PagedRequest request, Guid? zoneId = null)
        {
            var paged = await _deliveryRateRepository.GetPagedAsync(request, zoneId);
            return new PagedResult<DeliveryRateDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<DeliveryRateDto?> GetByIdAsync(Guid id)
        {
            var rate = await _deliveryRateRepository.GetByIdAsync(id);
            return rate == null ? null : MapToDto(rate);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<DeliveryRateDto> CreateAsync(CreateDeliveryRateRequest request)
        {
            var rate = new DeliveryRate
            {
                Id = Guid.NewGuid(),
                DeliveryZoneId = request.DeliveryZoneId,
                Name = request.Name,
                RateType = request.RateType.ToString(),
                FlatRateAmount = request.FlatRateAmount,
                PerKgRate = request.PerKgRate,
                MinWeight = request.MinWeight,
                MaxWeight = request.MaxWeight,
                WeightRangeRate = request.WeightRangeRate,
                MinOrderAmountForFree = request.MinOrderAmountForFree,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _deliveryRateRepository.CreateAsync(rate);

            // Reload with DeliveryZone included
            var loaded = await _deliveryRateRepository.GetByIdAsync(created.Id);
            return MapToDto(loaded!);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateDeliveryRateRequest request)
        {
            var rate = await _deliveryRateRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"DeliveryRate {id} not found.");

            if (request.DeliveryZoneId.HasValue) rate.DeliveryZoneId = request.DeliveryZoneId.Value;
            if (request.Name != null) rate.Name = request.Name;
            if (request.RateType.HasValue) rate.RateType = request.RateType.Value.ToString();
            if (request.FlatRateAmount.HasValue) rate.FlatRateAmount = request.FlatRateAmount.Value;
            if (request.PerKgRate.HasValue) rate.PerKgRate = request.PerKgRate.Value;
            if (request.MinWeight.HasValue) rate.MinWeight = request.MinWeight.Value;
            if (request.MaxWeight.HasValue) rate.MaxWeight = request.MaxWeight.Value;
            if (request.WeightRangeRate.HasValue) rate.WeightRangeRate = request.WeightRangeRate.Value;
            if (request.MinOrderAmountForFree.HasValue) rate.MinOrderAmountForFree = request.MinOrderAmountForFree.Value;
            if (request.IsActive.HasValue) rate.IsActive = request.IsActive.Value;

            await _deliveryRateRepository.UpdateAsync(rate);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var rate = await _deliveryRateRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"DeliveryRate {id} not found.");

            await _deliveryRateRepository.DeleteAsync(rate);
        }

        // ── Calculate Shipping ───────────────────────────────────────────────────

        public async Task<CalculateShippingResponse> CalculateShippingAsync(CalculateShippingRequest request)
        {
            var zone = await _deliveryZoneRepository.GetByIdAsync(request.DeliveryZoneId)
                ?? throw new KeyNotFoundException($"DeliveryZone {request.DeliveryZoneId} not found.");

            var rates = await _deliveryRateRepository.GetByZoneIdAsync(request.DeliveryZoneId);
            var rateList = rates.ToList();

            decimal shippingCost = 0;
            bool isFreeShipping = false;

            // Find the best matching rate
            var matchingRate = rateList.FirstOrDefault(r =>
                r.RateType == DeliveryRateType.WeightRange.ToString() &&
                request.TotalWeight >= r.MinWeight &&
                request.TotalWeight <= r.MaxWeight);

            if (matchingRate != null)
            {
                shippingCost = matchingRate.WeightRangeRate;
            }
            else
            {
                var perKgRate = rateList.FirstOrDefault(r => r.RateType == DeliveryRateType.PerKg.ToString());
                if (perKgRate != null)
                {
                    shippingCost = request.TotalWeight * perKgRate.PerKgRate;
                }
                else
                {
                    var flatRate = rateList.FirstOrDefault(r => r.RateType == DeliveryRateType.FlatRate.ToString());
                    if (flatRate != null)
                    {
                        shippingCost = flatRate.FlatRateAmount;
                    }
                }
            }

            // Check free shipping threshold
            var freeShippingRate = rateList.FirstOrDefault(r => r.MinOrderAmountForFree.HasValue && r.MinOrderAmountForFree.Value > 0);
            if (freeShippingRate != null && request.OrderAmount >= freeShippingRate.MinOrderAmountForFree!.Value)
            {
                shippingCost = 0;
                isFreeShipping = true;
            }

            return new CalculateShippingResponse
            {
                DeliveryZoneId = zone.Id,
                ZoneName = zone.Name,
                ShippingCost = shippingCost,
                EstimatedDeliveryDays = zone.EstimatedDeliveryDays,
                MaxDeliveryDays = zone.MaxDeliveryDays,
                IsFreeShipping = isFreeShipping
            };
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static DeliveryRateDto MapToDto(DeliveryRate r) => new()
        {
            Id = r.Id,
            DeliveryZoneId = r.DeliveryZoneId,
            DeliveryZoneName = r.DeliveryZone?.Name ?? string.Empty,
            Name = r.Name,
            RateType = r.RateType,
            FlatRateAmount = r.FlatRateAmount,
            PerKgRate = r.PerKgRate,
            MinWeight = r.MinWeight,
            MaxWeight = r.MaxWeight,
            WeightRangeRate = r.WeightRangeRate,
            MinOrderAmountForFree = r.MinOrderAmountForFree,
            IsActive = r.IsActive,
            CreatedAt = r.CreatedAt
        };
    }
}
