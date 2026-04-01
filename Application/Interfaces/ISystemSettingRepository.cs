using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISystemSettingRepository
    {
        Task<List<SystemSetting>> GetAllAsync(string? group = null);
        Task<SystemSetting?> GetByKeyAsync(string key);
        Task<SystemSetting?> GetByIdAsync(Guid id);
        Task<SystemSetting> CreateAsync(SystemSetting setting);
        Task UpdateAsync(SystemSetting setting);
    }
}
