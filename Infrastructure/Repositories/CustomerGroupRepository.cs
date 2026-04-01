using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CustomerGroupRepository : ICustomerGroupRepository
    {
        private readonly AppDbContext _context;

        public CustomerGroupRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<CustomerGroup>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.CustomerGroups.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(cg => cg.Name.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(cg => cg.Name) : query.OrderBy(cg => cg.Name),
                _ => query.OrderBy(cg => cg.Name)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<CustomerGroup?> GetByIdAsync(Guid id)
            => await _context.CustomerGroups.FirstOrDefaultAsync(cg => cg.Id == id);

        public async Task<bool> SlugExistsAsync(string slug)
            => await _context.CustomerGroups.AnyAsync(cg => cg.Slug == slug);

        public async Task<CustomerGroup> CreateAsync(CustomerGroup customerGroup)
        {
            _context.CustomerGroups.Add(customerGroup);
            await _context.SaveChangesAsync();
            return customerGroup;
        }

        public async Task UpdateAsync(CustomerGroup customerGroup)
        {
            _context.CustomerGroups.Update(customerGroup);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CustomerGroup customerGroup)
        {
            _context.CustomerGroups.Remove(customerGroup);
            await _context.SaveChangesAsync();
        }
    }
}
