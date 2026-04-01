using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IProductVariantRepository
    {
        Task<PagedResult<ProductVariant>> GetPagedAsync(PagedRequest request, Guid? productId = null);
        Task<IEnumerable<ProductVariant>> GetByProductIdAsync(Guid productId);
        Task<ProductVariant?> GetByIdAsync(Guid id);
        Task<bool> SkuExistsAsync(string sku);
        Task<bool> BarcodeExistsAsync(string barcode);
        Task<ProductVariant> CreateAsync(ProductVariant variant);
        Task UpdateAsync(ProductVariant variant);
        Task DeleteAsync(ProductVariant variant);
    }
}
