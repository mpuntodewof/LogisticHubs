using Domain.Entities;

namespace Application.Interfaces
{
    public interface INotificationPreferenceRepository
    {
        Task<NotificationPreference?> GetByUserIdAsync(Guid userId);
        Task<NotificationPreference> CreateAsync(NotificationPreference preference);
        Task UpdateAsync(NotificationPreference preference);
    }
}
