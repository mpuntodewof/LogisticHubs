using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class LoyaltyTierRepository : ILoyaltyTierRepository
    {
        private readonly AppDbContext _context;

        public LoyaltyTierRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LoyaltyTier>> GetByProgramIdAsync(Guid programId)
            => await _context.LoyaltyTiers
                .Where(t => t.LoyaltyProgramId == programId)
                .OrderBy(t => t.SortOrder)
                .ToListAsync();

        public async Task<LoyaltyTier?> GetByIdAsync(Guid id)
            => await _context.LoyaltyTiers.FirstOrDefaultAsync(t => t.Id == id);

        public async Task<LoyaltyTier> CreateAsync(LoyaltyTier tier)
        {
            _context.LoyaltyTiers.Add(tier);
            await _context.SaveChangesAsync();
            return tier;
        }

        public async Task UpdateAsync(LoyaltyTier tier)
        {
            _context.LoyaltyTiers.Update(tier);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(LoyaltyTier tier)
        {
            _context.LoyaltyTiers.Remove(tier);
            await _context.SaveChangesAsync();
        }

        public async Task<LoyaltyTier?> GetTierForPointsAsync(Guid programId, int lifetimePoints)
            => await _context.LoyaltyTiers
                .Where(t => t.LoyaltyProgramId == programId && t.MinPointsThreshold <= lifetimePoints)
                .OrderByDescending(t => t.MinPointsThreshold)
                .FirstOrDefaultAsync();
    }
}
