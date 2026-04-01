using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISupplierRepository
    {
        Task<PagedResult<Supplier>> GetPagedAsync(PagedRequest request, bool? isActive = null);
        Task<Supplier?> GetByIdAsync(Guid id);
        Task<bool> SupplierCodeExistsAsync(string supplierCode);
        Task<Supplier> CreateAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task DeleteAsync(Supplier supplier);
    }
}
