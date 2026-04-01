using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILoyaltyTierRepository
    {
        Task<IEnumerable<LoyaltyTier>> GetByProgramIdAsync(Guid programId);
        Task<LoyaltyTier?> GetByIdAsync(Guid id);
        Task<LoyaltyTier> CreateAsync(LoyaltyTier tier);
        Task UpdateAsync(LoyaltyTier tier);
        Task DeleteAsync(LoyaltyTier tier);
        Task<LoyaltyTier?> GetTierForPointsAsync(Guid programId, int lifetimePoints);
    }
}
