using Domain.Entities;

namespace Application.Interfaces
{
    public interface IProductImageRepository
    {
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(Guid productId);
        Task<ProductImage?> GetByIdAsync(Guid id);
        Task<ProductImage> CreateAsync(ProductImage image);
        Task DeleteAsync(ProductImage image);
    }
}
