using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class NotificationPreferenceRepository : INotificationPreferenceRepository
    {
        private readonly AppDbContext _context;

        public NotificationPreferenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationPreference?> GetByUserIdAsync(Guid userId)
            => await _context.NotificationPreferences.FirstOrDefaultAsync(p => p.UserId == userId);

        public async Task<NotificationPreference> CreateAsync(NotificationPreference preference)
        {
            _context.NotificationPreferences.Add(preference);
            await _context.SaveChangesAsync();
            return preference;
        }

        public async Task UpdateAsync(NotificationPreference preference)
        {
            _context.NotificationPreferences.Update(preference);
            await _context.SaveChangesAsync();
        }
    }
}
