using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICustomerAddressRepository
    {
        Task<IEnumerable<CustomerAddress>> GetByCustomerIdAsync(Guid customerId);
        Task<CustomerAddress?> GetByIdAsync(Guid id);
        Task<CustomerAddress> CreateAsync(CustomerAddress address);
        Task UpdateAsync(CustomerAddress address);
        Task DeleteAsync(CustomerAddress address);
        Task ClearDefaultsAsync(Guid customerId);
    }
}
