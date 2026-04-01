using Application.DTOs.Common;
using Application.DTOs.Logistics;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Logistics
{
    public class DeliveryZoneUseCase
    {
        private readonly IDeliveryZoneRepository _deliveryZoneRepository;

        public DeliveryZoneUseCase(IDeliveryZoneRepository deliveryZoneRepository)
        {
            _deliveryZoneRepository = deliveryZoneRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<DeliveryZoneDto>> GetPagedAsync(PagedRequest request)
        {
            var paged = await _deliveryZoneRepository.GetPagedAsync(request);
            return new PagedResult<DeliveryZoneDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<DeliveryZoneDto?> GetByIdAsync(Guid id)
        {
            var zone = await _deliveryZoneRepository.GetByIdAsync(id);
            return zone == null ? null : MapToDto(zone);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<DeliveryZoneDto> CreateAsync(CreateDeliveryZoneRequest request)
        {
            if (await _deliveryZoneRepository.CodeExistsAsync(request.Code))
                throw new InvalidOperationException($"A delivery zone with code '{request.Code}' already exists.");

            var zone = new DeliveryZone
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                CoverageAreas = request.CoverageAreas,
                Province = request.Province,
                EstimatedDeliveryDays = request.EstimatedDeliveryDays,
                MaxDeliveryDays = request.MaxDeliveryDays,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _deliveryZoneRepository.CreateAsync(zone);
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateDeliveryZoneRequest request)
        {
            var zone = await _deliveryZoneRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"DeliveryZone {id} not found.");

            if (request.Code != null)
            {
                if (await _deliveryZoneRepository.CodeExistsAsync(request.Code) &&
                    zone.Code != request.Code)
                    throw new InvalidOperationException($"A delivery zone with code '{request.Code}' already exists.");

                zone.Code = request.Code;
            }

            if (request.Name != null) zone.Name = request.Name;
            if (request.Description != null) zone.Description = request.Description;
            if (request.CoverageAreas != null) zone.CoverageAreas = request.CoverageAreas;
            if (request.Province != null) zone.Province = request.Province;
            if (request.EstimatedDeliveryDays.HasValue) zone.EstimatedDeliveryDays = request.EstimatedDeliveryDays.Value;
            if (request.MaxDeliveryDays.HasValue) zone.MaxDeliveryDays = request.MaxDeliveryDays.Value;
            if (request.IsActive.HasValue) zone.IsActive = request.IsActive.Value;

            await _deliveryZoneRepository.UpdateAsync(zone);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var zone = await _deliveryZoneRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"DeliveryZone {id} not found.");

            await _deliveryZoneRepository.DeleteAsync(zone);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static DeliveryZoneDto MapToDto(DeliveryZone z) => new()
        {
            Id = z.Id,
            Name = z.Name,
            Code = z.Code,
            Description = z.Description,
            CoverageAreas = z.CoverageAreas,
            Province = z.Province,
            EstimatedDeliveryDays = z.EstimatedDeliveryDays,
            MaxDeliveryDays = z.MaxDeliveryDays,
            IsActive = z.IsActive,
            CreatedAt = z.CreatedAt
        };
    }
}
