using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICustomerRepository
    {
        Task<PagedResult<Customer>> GetPagedAsync(PagedRequest request);
        Task<Customer?> GetByIdAsync(Guid id);
        Task<Customer?> GetDetailByIdAsync(Guid id);
        Task<bool> CustomerCodeExistsAsync(string customerCode);
        Task<Customer> CreateAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Customer customer);
    }
}
