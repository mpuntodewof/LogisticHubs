using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILoyaltyProgramRepository
    {
        Task<PagedResult<LoyaltyProgram>> GetPagedAsync(PagedRequest request);
        Task<LoyaltyProgram?> GetByIdAsync(Guid id);
        Task<LoyaltyProgram?> GetDetailByIdAsync(Guid id);
        Task<IEnumerable<LoyaltyProgram>> GetActiveAsync();
        Task<LoyaltyProgram> CreateAsync(LoyaltyProgram program);
        Task UpdateAsync(LoyaltyProgram program);
        Task DeleteAsync(LoyaltyProgram program);
    }
}
