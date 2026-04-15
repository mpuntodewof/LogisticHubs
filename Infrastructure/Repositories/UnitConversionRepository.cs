using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UnitConversionRepository : IUnitConversionRepository
    {
        private readonly AppDbContext _context;

        public UnitConversionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UnitConversion>> GetByFromUnitAsync(Guid fromUnitId)
            => await _context.UnitConversions
                .Include(uc => uc.FromUnit)
                .Include(uc => uc.ToUnit)
                .Where(uc => uc.FromUnitId == fromUnitId)
                .ToListAsync();

        public async Task<UnitConversion?> GetByIdAsync(Guid id)
            => await _context.UnitConversions
                .Include(uc => uc.FromUnit)
                .Include(uc => uc.ToUnit)
                .FirstOrDefaultAsync(uc => uc.Id == id);

        public async Task<bool> ExistsAsync(Guid fromUnitId, Guid toUnitId)
            => await _context.UnitConversions.AnyAsync(uc => uc.FromUnitId == fromUnitId && uc.ToUnitId == toUnitId);

        public async Task<UnitConversion> CreateAsync(UnitConversion conversion)
        {
            _context.UnitConversions.Add(conversion);
            return conversion;
        }

        public async Task UpdateAsync(UnitConversion conversion)
        {
            _context.UnitConversions.Update(conversion);
        }

        public async Task DeleteAsync(UnitConversion conversion)
        {
            _context.UnitConversions.Remove(conversion);
        }
    }
}
