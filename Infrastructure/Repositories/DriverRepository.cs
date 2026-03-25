using Application.Interfaces;
using Domain.Entities;
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
