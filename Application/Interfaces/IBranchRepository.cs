using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IBranchRepository
    {
        Task<PagedResult<Branch>> GetPagedAsync(PagedRequest request);
        Task<Branch?> GetByIdAsync(Guid id);
        Task<bool> CodeExistsAsync(string code);
        Task<Branch> CreateAsync(Branch branch);
        Task UpdateAsync(Branch branch);
        Task DeleteAsync(Branch branch);
        Task<IEnumerable<BranchUser>> GetBranchUsersAsync(Guid branchId);
        Task AssignUserAsync(BranchUser branchUser);
        Task RemoveUserAsync(Guid branchId, Guid userId);
        Task<IEnumerable<BranchUser>> GetBranchesByUserIdAsync(Guid userId);
    }
}
