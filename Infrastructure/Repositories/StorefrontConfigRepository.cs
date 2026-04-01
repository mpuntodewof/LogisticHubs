using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StorefrontConfigRepository : IStorefrontConfigRepository
    {
        private readonly AppDbContext _context;

        public StorefrontConfigRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StorefrontConfig?> GetByTenantAsync()
            => await _context.StorefrontConfigs.FirstOrDefaultAsync();

        public async Task<StorefrontConfig> CreateAsync(StorefrontConfig config)
        {
            _context.StorefrontConfigs.Add(config);
            await _context.SaveChangesAsync();
            return config;
        }

        public async Task UpdateAsync(StorefrontConfig config)
        {
            _context.StorefrontConfigs.Update(config);
            await _context.SaveChangesAsync();
        }
    }
}
