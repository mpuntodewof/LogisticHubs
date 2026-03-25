using Application.Interfaces;
using Domain.Entities;
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
