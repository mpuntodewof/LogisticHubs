using Application.DTOs.Common;
using Application.DTOs.Inventory;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Inventory
{
    public class WarehouseStockUseCase
    {
        private readonly IWarehouseStockRepository _stockRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseStockUseCase(IWarehouseStockRepository stockRepository, IUnitOfWork unitOfWork)
        {
            _stockRepository = stockRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<WarehouseStockDto>> GetPagedAsync(
            PagedRequest request, Guid? warehouseId = null, Guid? productVariantId = null)
        {
            var result = await _stockRepository.GetPagedAsync(request, warehouseId, productVariantId);

            return new PagedResult<WarehouseStockDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<IEnumerable<WarehouseStockDto>> GetLowStockAsync(Guid? warehouseId = null)
        {
            var stocks = await _stockRepository.GetLowStockAsync(warehouseId);
            return stocks.Select(MapToDto);
        }

        public async Task UpdateSettingsAsync(Guid id, UpdateWarehouseStockRequest request)
        {
            var stock = await _stockRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Warehouse stock not found.");

            if (request.ReorderPoint.HasValue) stock.ReorderPoint = request.ReorderPoint.Value;
            if (request.MaxStock.HasValue) stock.MaxStock = request.MaxStock.Value;

            await _stockRepository.UpdateAsync(stock);
            await _unitOfWork.SaveChangesAsync();
        }

        private static WarehouseStockDto MapToDto(WarehouseStock s) => new()
        {
            Id = s.Id,
            WarehouseId = s.WarehouseId,
            WarehouseName = s.Warehouse?.Name ?? string.Empty,
            ProductVariantId = s.ProductVariantId,
            Sku = s.ProductVariant?.Sku ?? string.Empty,
            ProductVariantName = s.ProductVariant?.Name ?? string.Empty,
            QuantityOnHand = s.QuantityOnHand,
            QuantityReserved = s.QuantityReserved,
            QuantityAvailable = s.QuantityAvailable,
            ReorderPoint = s.ReorderPoint,
            MaxStock = s.MaxStock
        };
    }
}
