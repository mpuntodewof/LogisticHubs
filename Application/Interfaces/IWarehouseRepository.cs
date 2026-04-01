using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IWarehouseRepository
    {
        Task<IEnumerable<Warehouse>> GetAllAsync();
        Task<PagedResult<Warehouse>> GetPagedAsync(PagedRequest request);
        Task<Warehouse?> GetByIdAsync(Guid id);
        Task<bool> NameExistsAsync(string name);
        Task<Warehouse> CreateAsync(Warehouse warehouse);
        Task UpdateAsync(Warehouse warehouse);
        Task DeleteAsync(Warehouse warehouse);
    }
}
