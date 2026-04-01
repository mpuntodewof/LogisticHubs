using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly AppDbContext _context;

        public DriverRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Driver>> GetAllAsync()
            => await _context.Drivers.ToListAsync();

        public async Task<PagedResult<Driver>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.Drivers.Include(d => d.User).AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(d => d.LicenseNumber.ToLower().Contains(search) || d.User.Name.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "license" => request.SortDescending ? query.OrderByDescending(d => d.LicenseNumber) : query.OrderBy(d => d.LicenseNumber),
                "status" => request.SortDescending ? query.OrderByDescending(d => d.Status) : query.OrderBy(d => d.Status),
                _ => query.OrderByDescending(d => d.CreatedAt)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<Driver?> GetByIdAsync(Guid id)
            => await _context.Drivers.FindAsync(id);

        public async Task<Driver?> GetByUserIdAsync(Guid userId)
            => await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == userId);

        public async Task<bool> LicenseNumberExistsAsync(string licenseNumber)
            => await _context.Drivers.AnyAsync(d => d.LicenseNumber == licenseNumber);

        public async Task<Driver> CreateAsync(Driver driver)
        {
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();
            return driver;
        }

        public async Task UpdateAsync(Driver driver)
        {
            _context.Drivers.Update(driver);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Driver driver)
        {
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
        }
    }
}
