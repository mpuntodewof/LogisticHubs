using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly AppDbContext _context;

        public WarehouseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Warehouse>> GetAllAsync()
            => await _context.Warehouses.OrderBy(w => w.Name).ToListAsync();

        public async Task<Warehouse?> GetByIdAsync(Guid id)
            => await _context.Warehouses.FindAsync(id);

        public async Task<bool> NameExistsAsync(string name)
            => await _context.Warehouses.AnyAsync(w => w.Name == name);

        public async Task<Warehouse> CreateAsync(Warehouse warehouse)
        {
            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();
            return warehouse;
        }

        public async Task UpdateAsync(Warehouse warehouse)
        {
            _context.Warehouses.Update(warehouse);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Warehouse warehouse)
        {
            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();
        }
    }
}
