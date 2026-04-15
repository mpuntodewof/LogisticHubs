using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SystemSettingRepository : ISystemSettingRepository
    {
        private readonly AppDbContext _context;

        public SystemSettingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SystemSetting>> GetAllAsync(string? group = null)
        {
            var query = _context.SystemSettings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(group))
                query = query.Where(s => s.Group == group);

            return await query
                .OrderBy(s => s.Group)
                .ThenBy(s => s.Key)
                .ToListAsync();
        }

        public async Task<SystemSetting?> GetByKeyAsync(string key)
            => await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key);

        public async Task<SystemSetting?> GetByIdAsync(Guid id)
            => await _context.SystemSettings.FirstOrDefaultAsync(s => s.Id == id);

        public async Task<SystemSetting> CreateAsync(SystemSetting setting)
        {
            _context.SystemSettings.Add(setting);
            return setting;
        }

        public async Task UpdateAsync(SystemSetting setting)
        {
            _context.SystemSettings.Update(setting);
        }
    }
}
