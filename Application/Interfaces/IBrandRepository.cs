using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IBrandRepository
    {
        Task<IEnumerable<Brand>> GetAllAsync();
        Task<PagedResult<Brand>> GetPagedAsync(PagedRequest request);
        Task<Brand?> GetByIdAsync(Guid id);
        Task<bool> SlugExistsAsync(string slug);
        Task<Brand> CreateAsync(Brand brand);
        Task UpdateAsync(Brand brand);
        Task DeleteAsync(Brand brand);
    }
}
