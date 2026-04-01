using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AppDbContext _context;

        public VehicleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
            => await _context.Vehicles.OrderBy(v => v.PlateNumber).ToListAsync();

        public async Task<PagedResult<Vehicle>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.Vehicles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(v => v.PlateNumber.ToLower().Contains(search) || v.VehicleType.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "plate" => request.SortDescending ? query.OrderByDescending(v => v.PlateNumber) : query.OrderBy(v => v.PlateNumber),
                "type" => request.SortDescending ? query.OrderByDescending(v => v.VehicleType) : query.OrderBy(v => v.VehicleType),
                "status" => request.SortDescending ? query.OrderByDescending(v => v.Status) : query.OrderBy(v => v.Status),
                _ => query.OrderByDescending(v => v.CreatedAt)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<Vehicle?> GetByIdAsync(Guid id)
            => await _context.Vehicles.FindAsync(id);

        public async Task<bool> PlateNumberExistsAsync(string plateNumber)
            => await _context.Vehicles.AnyAsync(v => v.PlateNumber == plateNumber.ToUpperInvariant());

        public async Task<Vehicle> CreateAsync(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task UpdateAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }
    }
}
