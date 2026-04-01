using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Notification>> GetPagedByUserAsync(Guid userId, PagedRequest request, string? status)
        {
            var query = _context.Notifications.Where(n => n.UserId == userId);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(n => n.Status == status);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(search) || n.Message.ToLower().Contains(search));
            }

            query = query.OrderByDescending(n => n.CreatedAt);

            return await query.ToPagedResultAsync(request);
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
            => await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);

        public async Task<int> GetUnreadCountAsync(Guid userId)
            => await _context.Notifications.CountAsync(n => n.UserId == userId && n.Status == "Unread");

        public async Task<Notification> CreateAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAllReadAsync(Guid userId)
        {
            await _context.Notifications
                .Where(n => n.UserId == userId && n.Status == "Unread")
                .ExecuteUpdateAsync(s => s
                    .SetProperty(n => n.Status, "Read")
                    .SetProperty(n => n.ReadAt, DateTime.UtcNow));
        }

        public async Task DeleteAsync(Notification notification)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }
}
