using Domain.Entities;

namespace Application.Interfaces
{
    public interface IStorefrontConfigRepository
    {
        Task<StorefrontConfig?> GetByTenantAsync();
        Task<StorefrontConfig> CreateAsync(StorefrontConfig config);
        Task UpdateAsync(StorefrontConfig config);
    }
}
