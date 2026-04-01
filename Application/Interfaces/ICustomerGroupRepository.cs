using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICustomerGroupRepository
    {
        Task<PagedResult<CustomerGroup>> GetPagedAsync(PagedRequest request);
        Task<CustomerGroup?> GetByIdAsync(Guid id);
        Task<bool> SlugExistsAsync(string slug);
        Task<CustomerGroup> CreateAsync(CustomerGroup customerGroup);
        Task UpdateAsync(CustomerGroup customerGroup);
        Task DeleteAsync(CustomerGroup customerGroup);
    }
}
