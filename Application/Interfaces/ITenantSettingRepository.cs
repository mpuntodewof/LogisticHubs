using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITenantSettingRepository
    {
        Task<List<TenantSetting>> GetAllAsync(string? group = null);
        Task<TenantSetting?> GetByKeyAsync(string key);
        Task<TenantSetting?> GetByIdAsync(Guid id);
        Task<TenantSetting> CreateAsync(TenantSetting setting);
        Task UpdateAsync(TenantSetting setting);
    }
}
