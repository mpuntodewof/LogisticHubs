using Application.DTOs.Common;
using Application.DTOs.Drivers;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Drivers
{
    public class DriverUseCase
    {
        private readonly IDriverRepository _driverRepository;

        public DriverUseCase(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<DriverDto>> GetAllAsync()
        {
            var drivers = await _driverRepository.GetAllAsync();
            return drivers.Select(MapToDto);
        }

        public async Task<PagedResult<DriverDto>> GetPagedAsync(PagedRequest request)
        {
            var paged = await _driverRepository.GetPagedAsync(request);
            return new PagedResult<DriverDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<DriverDto?> GetByIdAsync(Guid id)
        {
            var driver = await _driverRepository.GetByIdAsync(id);
            return driver == null ? null : MapToDto(driver);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<DriverDto> CreateAsync(CreateDriverRequest request)
        {
            if (await _driverRepository.GetByUserIdAsync(request.UserId) != null)
                throw new InvalidOperationException("This user is already registered as a driver.");

            if (await _driverRepository.LicenseNumberExistsAsync(request.LicenseNumber))
                throw new InvalidOperationException("A driver with this license number already exists.");

            var driver = new Driver
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                LicenseNumber = request.LicenseNumber,
                Phone = request.Phone,
                Status = DriverStatus.Available.ToString()
            };

            var created = await _driverRepository.CreateAsync(driver);
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateDriverRequest request)
        {
            var driver = await _driverRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Driver {id} not found.");

            if (request.LicenseNumber != null)
            {
                if (await _driverRepository.LicenseNumberExistsAsync(request.LicenseNumber) &&
                    driver.LicenseNumber != request.LicenseNumber)
                    throw new InvalidOperationException("A driver with this license number already exists.");

                driver.LicenseNumber = request.LicenseNumber;
            }

            if (request.Phone != null) driver.Phone = request.Phone;
            if (request.Status.HasValue) driver.Status = request.Status.Value.ToString();

            await _driverRepository.UpdateAsync(driver);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var driver = await _driverRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Driver {id} not found.");

            if (driver.Status == DriverStatus.OnDuty.ToString())
                throw new InvalidOperationException("Cannot delete a driver who is currently on duty.");

            await _driverRepository.DeleteAsync(driver);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static DriverDto MapToDto(Driver d) => new()
        {
            Id = d.Id,
            UserId = d.UserId,
            LicenseNumber = d.LicenseNumber,
            Phone = d.Phone,
            Status = d.Status
        };
    }
}
