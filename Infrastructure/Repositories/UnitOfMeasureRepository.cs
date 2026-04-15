using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UnitOfMeasureRepository : IUnitOfMeasureRepository
    {
        private readonly AppDbContext _context;

        public UnitOfMeasureRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UnitOfMeasure>> GetAllAsync()
            => await _context.UnitsOfMeasure.OrderBy(u => u.Name).ToListAsync();

        public async Task<PagedResult<UnitOfMeasure>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.UnitsOfMeasure.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(u => u.Name.ToLower().Contains(search) || u.Abbreviation.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),
                "abbreviation" => request.SortDescending ? query.OrderByDescending(u => u.Abbreviation) : query.OrderBy(u => u.Abbreviation),
                _ => query.OrderBy(u => u.Name)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<UnitOfMeasure?> GetByIdAsync(Guid id)
            => await _context.UnitsOfMeasure.FindAsync(id);

        public async Task<bool> AbbreviationExistsAsync(string abbreviation)
            => await _context.UnitsOfMeasure.AnyAsync(u => u.Abbreviation == abbreviation);

        public async Task<bool> IsInUseByProductsAsync(Guid id)
            => await _context.Products.AnyAsync(p => p.BaseUnitOfMeasureId == id);

        public async Task<UnitOfMeasure> CreateAsync(UnitOfMeasure unit)
        {
            _context.UnitsOfMeasure.Add(unit);
            return unit;
        }

        public async Task UpdateAsync(UnitOfMeasure unit)
        {
            _context.UnitsOfMeasure.Update(unit);
        }

        public async Task DeleteAsync(UnitOfMeasure unit)
        {
            _context.UnitsOfMeasure.Remove(unit);
        }
    }
}
