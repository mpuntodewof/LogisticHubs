using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
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

        public async Task<PagedResult<Warehouse>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.Warehouses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(w => w.Name.ToLower().Contains(search) || w.Location.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(w => w.Name) : query.OrderBy(w => w.Name),
                "location" => request.SortDescending ? query.OrderByDescending(w => w.Location) : query.OrderBy(w => w.Location),
                _ => query.OrderByDescending(w => w.CreatedAt)
            };

            return await query.ToPagedResultAsync(request);
        }

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
