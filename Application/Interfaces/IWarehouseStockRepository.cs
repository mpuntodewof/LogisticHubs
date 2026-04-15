using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IWarehouseStockRepository
    {
        Task<PagedResult<WarehouseStock>> GetPagedAsync(PagedRequest request, Guid? warehouseId = null, Guid? productVariantId = null);
        Task<WarehouseStock?> GetByIdAsync(Guid id);
        Task<WarehouseStock?> GetByWarehouseAndVariantAsync(Guid warehouseId, Guid productVariantId);
        Task<IEnumerable<WarehouseStock>> GetLowStockAsync(Guid? warehouseId = null);
        Task<WarehouseStock> CreateAsync(WarehouseStock stock);
        Task UpdateAsync(WarehouseStock stock);
        Task ReloadAsync(WarehouseStock stock);
    }
}
