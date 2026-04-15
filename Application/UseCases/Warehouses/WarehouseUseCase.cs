using Application.DTOs.Common;
using Application.DTOs.Warehouses;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Warehouses
{
    public class WarehouseUseCase
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseUseCase(IWarehouseRepository warehouseRepository, IUnitOfWork unitOfWork)
        {
            _warehouseRepository = warehouseRepository;
            _unitOfWork = unitOfWork;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<WarehouseDto>> GetAllAsync()
        {
            var warehouses = await _warehouseRepository.GetAllAsync();
            return warehouses.Select(MapToDto);
        }

        public async Task<PagedResult<WarehouseDto>> GetPagedAsync(PagedRequest request)
        {
            var paged = await _warehouseRepository.GetPagedAsync(request);
            return new PagedResult<WarehouseDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<WarehouseDto?> GetByIdAsync(Guid id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            return warehouse == null ? null : MapToDto(warehouse);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<WarehouseDto> CreateAsync(CreateWarehouseRequest request)
        {
            if (await _warehouseRepository.NameExistsAsync(request.Name))
                throw new InvalidOperationException($"A warehouse named '{request.Name}' already exists.");

            var warehouse = new Warehouse
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Location = request.Location,
                Capacity = request.Capacity,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _warehouseRepository.CreateAsync(warehouse);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateWarehouseRequest request)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Warehouse {id} not found.");

            if (request.Name != null)
            {
                if (await _warehouseRepository.NameExistsAsync(request.Name) &&
                    warehouse.Name != request.Name)
                    throw new InvalidOperationException($"A warehouse named '{request.Name}' already exists.");

                warehouse.Name = request.Name;
            }

            if (request.Location != null) warehouse.Location = request.Location;
            if (request.Capacity.HasValue) warehouse.Capacity = request.Capacity.Value;

            await _warehouseRepository.UpdateAsync(warehouse);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Warehouse {id} not found.");

            await _warehouseRepository.DeleteAsync(warehouse);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static WarehouseDto MapToDto(Warehouse w) => new()
        {
            Id = w.Id,
            Name = w.Name,
            Location = w.Location,
            Capacity = w.Capacity,
            CreatedAt = w.CreatedAt
        };
    }
}
