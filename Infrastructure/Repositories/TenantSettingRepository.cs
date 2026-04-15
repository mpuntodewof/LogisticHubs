using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TenantSettingRepository : ITenantSettingRepository
    {
        private readonly AppDbContext _context;

        public TenantSettingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TenantSetting>> GetAllAsync(string? group = null)
        {
            var query = _context.TenantSettings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(group))
                query = query.Where(s => s.Group == group);

            return await query
                .OrderBy(s => s.Group)
                .ThenBy(s => s.Key)
                .ToListAsync();
        }

        public async Task<TenantSetting?> GetByKeyAsync(string key)
            => await _context.TenantSettings.FirstOrDefaultAsync(s => s.Key == key);

        public async Task<TenantSetting?> GetByIdAsync(Guid id)
            => await _context.TenantSettings.FirstOrDefaultAsync(s => s.Id == id);

        public async Task<TenantSetting> CreateAsync(TenantSetting setting)
        {
            _context.TenantSettings.Add(setting);
            return setting;
        }

        public async Task UpdateAsync(TenantSetting setting)
        {
            _context.TenantSettings.Update(setting);
        }
    }
}
