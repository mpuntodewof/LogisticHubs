using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface INotificationRepository
    {
        Task<PagedResult<Notification>> GetPagedByUserAsync(Guid userId, PagedRequest request, string? status);
        Task<Notification?> GetByIdAsync(Guid id);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task<Notification> CreateAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task MarkAllReadAsync(Guid userId);
        Task DeleteAsync(Notification notification);
    }
}
