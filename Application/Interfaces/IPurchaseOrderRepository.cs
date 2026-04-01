using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        Task<PagedResult<PurchaseOrder>> GetPagedAsync(PagedRequest request, string? status = null, Guid? supplierId = null, Guid? warehouseId = null);
        Task<PurchaseOrder?> GetByIdAsync(Guid id);
        Task<PurchaseOrder?> GetDetailByIdAsync(Guid id);
        Task<bool> PoNumberExistsAsync(string poNumber);
        Task<PurchaseOrder> CreateAsync(PurchaseOrder order);
        Task UpdateAsync(PurchaseOrder order);
        Task DeleteAsync(PurchaseOrder order);
    }
}
