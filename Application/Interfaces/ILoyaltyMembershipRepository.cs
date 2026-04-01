using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILoyaltyMembershipRepository
    {
        Task<PagedResult<LoyaltyMembership>> GetPagedAsync(PagedRequest request, Guid? programId = null, Guid? customerId = null);
        Task<LoyaltyMembership?> GetByIdAsync(Guid id);
        Task<LoyaltyMembership?> GetByCustomerAndProgramAsync(Guid customerId, Guid programId);
        Task<LoyaltyMembership> CreateAsync(LoyaltyMembership membership);
        Task UpdateAsync(LoyaltyMembership membership);
    }
}
