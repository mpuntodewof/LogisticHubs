using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DeliveryZoneRepository : IDeliveryZoneRepository
    {
        private readonly AppDbContext _context;

        public DeliveryZoneRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<DeliveryZone>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.DeliveryZones.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(z => z.Name.ToLower().Contains(search)
                    || z.Code.ToLower().Contains(search)
                    || (z.Province != null && z.Province.ToLower().Contains(search)));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(z => z.Name) : query.OrderBy(z => z.Name),
                "code" => request.SortDescending ? query.OrderByDescending(z => z.Code) : query.OrderBy(z => z.Code),
                _ => query.OrderBy(z => z.Name)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<DeliveryZone?> GetByIdAsync(Guid id)
            => await _context.DeliveryZones.FindAsync(id);

        public async Task<bool> CodeExistsAsync(string code)
            => await _context.DeliveryZones.AnyAsync(z => z.Code == code);

        public async Task<DeliveryZone> CreateAsync(DeliveryZone zone)
        {
            _context.DeliveryZones.Add(zone);
            await _context.SaveChangesAsync();
            return zone;
        }

        public async Task UpdateAsync(DeliveryZone zone)
        {
            _context.DeliveryZones.Update(zone);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(DeliveryZone zone)
        {
            _context.DeliveryZones.Remove(zone);
            await _context.SaveChangesAsync();
        }
    }
}
