using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DeliveryRateRepository : IDeliveryRateRepository
    {
        private readonly AppDbContext _context;

        public DeliveryRateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<DeliveryRate>> GetPagedAsync(PagedRequest request, Guid? zoneId = null)
        {
            var query = _context.DeliveryRates
                .Include(r => r.DeliveryZone)
                .AsQueryable();

            if (zoneId.HasValue)
                query = query.Where(r => r.DeliveryZoneId == zoneId.Value);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(r => r.Name.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name),
                _ => query.OrderBy(r => r.Name)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<DeliveryRate?> GetByIdAsync(Guid id)
            => await _context.DeliveryRates
                .Include(r => r.DeliveryZone)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<IEnumerable<DeliveryRate>> GetByZoneIdAsync(Guid zoneId)
            => await _context.DeliveryRates
                .Where(r => r.DeliveryZoneId == zoneId && r.IsActive)
                .ToListAsync();

        public async Task<DeliveryRate> CreateAsync(DeliveryRate rate)
        {
            _context.DeliveryRates.Add(rate);
            await _context.SaveChangesAsync();
            return rate;
        }

        public async Task UpdateAsync(DeliveryRate rate)
        {
            _context.DeliveryRates.Update(rate);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(DeliveryRate rate)
        {
            _context.DeliveryRates.Remove(rate);
            await _context.SaveChangesAsync();
        }
    }
}
