using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IProductRepository
    {
        Task<PagedResult<Product>> GetPagedAsync(PagedRequest request);
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product?> GetDetailByIdAsync(Guid id);
        Task<bool> SlugExistsAsync(string slug);
        Task<Product> CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}
