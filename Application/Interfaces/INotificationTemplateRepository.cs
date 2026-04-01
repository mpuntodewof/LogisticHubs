using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface INotificationTemplateRepository
    {
        Task<PagedResult<NotificationTemplate>> GetPagedAsync(PagedRequest request);
        Task<NotificationTemplate?> GetByIdAsync(Guid id);
        Task<NotificationTemplate?> GetByCodeAsync(string code);
        Task<bool> CodeExistsAsync(string code);
        Task<NotificationTemplate> CreateAsync(NotificationTemplate template);
        Task UpdateAsync(NotificationTemplate template);
        Task DeleteAsync(NotificationTemplate template);
    }
}
