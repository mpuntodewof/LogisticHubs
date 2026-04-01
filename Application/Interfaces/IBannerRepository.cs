using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IBannerRepository
    {
        Task<PagedResult<Banner>> GetPagedAsync(PagedRequest request, string? position = null, bool? isActive = null);
        Task<Banner?> GetByIdAsync(Guid id);
        Task<IEnumerable<Banner>> GetActiveBannersAsync(string? position = null);
        Task<Banner> CreateAsync(Banner banner);
        Task UpdateAsync(Banner banner);
        Task DeleteAsync(Banner banner);
    }
}
