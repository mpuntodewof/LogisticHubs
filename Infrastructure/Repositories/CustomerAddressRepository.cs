using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CustomerAddressRepository : ICustomerAddressRepository
    {
        private readonly AppDbContext _context;

        public CustomerAddressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerAddress>> GetByCustomerIdAsync(Guid customerId)
            => await _context.CustomerAddresses
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.Label)
                .ToListAsync();

        public async Task<CustomerAddress?> GetByIdAsync(Guid id)
            => await _context.CustomerAddresses.FirstOrDefaultAsync(a => a.Id == id);

        public async Task<CustomerAddress> CreateAsync(CustomerAddress address)
        {
            _context.CustomerAddresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task UpdateAsync(CustomerAddress address)
        {
            _context.CustomerAddresses.Update(address);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CustomerAddress address)
        {
            _context.CustomerAddresses.Remove(address);
            await _context.SaveChangesAsync();
        }

        public async Task ClearDefaultsAsync(Guid customerId)
        {
            await _context.CustomerAddresses
                .Where(a => a.CustomerId == customerId && a.IsDefault)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDefault, false));
        }
    }
}
