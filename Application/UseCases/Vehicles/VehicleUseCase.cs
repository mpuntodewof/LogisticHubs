using Application.DTOs.Common;
using Application.DTOs.Vehicles;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Vehicles
{
    public class VehicleUseCase
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleUseCase(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<VehicleDto>> GetAllAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return vehicles.Select(MapToDto);
        }

        public async Task<PagedResult<VehicleDto>> GetPagedAsync(PagedRequest request)
        {
            var paged = await _vehicleRepository.GetPagedAsync(request);
            return new PagedResult<VehicleDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<VehicleDto?> GetByIdAsync(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            return vehicle == null ? null : MapToDto(vehicle);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<VehicleDto> CreateAsync(CreateVehicleRequest request)
        {
            if (await _vehicleRepository.PlateNumberExistsAsync(request.PlateNumber))
                throw new InvalidOperationException($"A vehicle with plate number '{request.PlateNumber}' already exists.");

            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                PlateNumber = request.PlateNumber.ToUpperInvariant(),
                VehicleType = request.VehicleType,
                CapacityWeight = request.CapacityWeight,
                CapacityVolume = request.CapacityVolume,
                Status = VehicleStatus.Available.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            var created = await _vehicleRepository.CreateAsync(vehicle);
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateVehicleRequest request)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Vehicle {id} not found.");

            if (request.VehicleType != null) vehicle.VehicleType = request.VehicleType;
            if (request.CapacityWeight.HasValue) vehicle.CapacityWeight = request.CapacityWeight.Value;
            if (request.CapacityVolume.HasValue) vehicle.CapacityVolume = request.CapacityVolume.Value;
            if (request.Status.HasValue) vehicle.Status = request.Status.Value.ToString();

            await _vehicleRepository.UpdateAsync(vehicle);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Vehicle {id} not found.");

            if (vehicle.Status == VehicleStatus.InUse.ToString())
                throw new InvalidOperationException("Cannot delete a vehicle that is currently in use.");

            await _vehicleRepository.DeleteAsync(vehicle);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static VehicleDto MapToDto(Vehicle v) => new()
        {
            Id = v.Id,
            PlateNumber = v.PlateNumber,
            VehicleType = v.VehicleType,
            CapacityWeight = v.CapacityWeight,
            CapacityVolume = v.CapacityVolume,
            Status = v.Status,
            CreatedAt = v.CreatedAt
        };
    }
}
