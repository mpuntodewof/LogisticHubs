using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IApiKeyRepository
    {
        Task<PagedResult<ApiKey>> GetPagedAsync(PagedRequest request, bool? isActive = null);
        Task<ApiKey?> GetByIdAsync(Guid id);
        Task<ApiKey?> GetByKeyHashAsync(string keyHash);
        Task<bool> NameExistsAsync(string name);
        Task<ApiKey> CreateAsync(ApiKey apiKey);
        Task UpdateAsync(ApiKey apiKey);
        Task DeleteAsync(ApiKey apiKey);
    }
}
