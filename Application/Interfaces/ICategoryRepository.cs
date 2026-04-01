using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<PagedResult<Category>> GetPagedAsync(PagedRequest request);
        Task<IEnumerable<Category>> GetTreeAsync();
        Task<Category?> GetByIdAsync(Guid id);
        Task<bool> SlugExistsAsync(string slug);
        Task<bool> HasChildrenAsync(Guid id);
        Task<bool> HasProductsAsync(Guid id);
        Task<Category> CreateAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
}
