using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITaxRateRepository
    {
        Task<PagedResult<TaxRate>> GetPagedAsync(PagedRequest request);
        Task<TaxRate?> GetByIdAsync(Guid id);
        Task<bool> CodeExistsAsync(string code);
        Task<IEnumerable<TaxRate>> GetActiveByProductIdAsync(Guid productId);
        Task<TaxRate> CreateAsync(TaxRate taxRate);
        Task UpdateAsync(TaxRate taxRate);
        Task DeleteAsync(TaxRate taxRate);
        Task AssignToProductAsync(Guid productId, Guid taxRateId, Guid tenantId);
        Task RemoveFromProductAsync(Guid productId, Guid taxRateId);
        Task<IEnumerable<ProductTaxRate>> GetProductTaxRatesAsync(Guid productId);
    }
}
