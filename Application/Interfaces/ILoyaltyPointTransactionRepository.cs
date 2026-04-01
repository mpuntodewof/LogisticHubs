using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILoyaltyPointTransactionRepository
    {
        Task<PagedResult<LoyaltyPointTransaction>> GetByMembershipIdAsync(Guid membershipId, PagedRequest request);
        Task<LoyaltyPointTransaction> CreateAsync(LoyaltyPointTransaction transaction);
    }
}
