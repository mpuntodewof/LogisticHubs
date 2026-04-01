using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Customer>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.Customers.Include(c => c.CustomerGroup).AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(search) ||
                    c.CustomerCode.ToLower().Contains(search) ||
                    (c.Email != null && c.Email.ToLower().Contains(search)));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                "code" => request.SortDescending ? query.OrderByDescending(c => c.CustomerCode) : query.OrderBy(c => c.CustomerCode),
                _ => query.OrderByDescending(c => c.CreatedAt)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
            => await _context.Customers.Include(c => c.CustomerGroup).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Customer?> GetDetailByIdAsync(Guid id)
            => await _context.Customers
                .Include(c => c.CustomerGroup)
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<bool> CustomerCodeExistsAsync(string customerCode)
            => await _context.Customers.AnyAsync(c => c.CustomerCode == customerCode);

        public async Task<Customer> CreateAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Customer customer)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}
