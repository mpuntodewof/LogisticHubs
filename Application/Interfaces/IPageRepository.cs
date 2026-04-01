using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPageRepository
    {
        Task<PagedResult<Page>> GetPagedAsync(PagedRequest request, string? status = null);
        Task<Page?> GetByIdAsync(Guid id);
        Task<Page?> GetBySlugAsync(string slug);
        Task<bool> SlugExistsAsync(string slug);
        Task<Page> CreateAsync(Page page);
        Task UpdateAsync(Page page);
        Task DeleteAsync(Page page);
    }
}
