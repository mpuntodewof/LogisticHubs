using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IStockMovementRepository
    {
        Task<PagedResult<StockMovement>> GetPagedAsync(PagedRequest request, Guid? warehouseId = null, Guid? productVariantId = null, string? movementType = null);
        Task<StockMovement?> GetByIdAsync(Guid id);
        Task<StockMovement> CreateAsync(StockMovement movement);
    }
}
