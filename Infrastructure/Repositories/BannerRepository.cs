using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BannerRepository : IBannerRepository
    {
        private readonly AppDbContext _context;

        public BannerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Banner>> GetPagedAsync(PagedRequest request, string? position = null, bool? isActive = null)
        {
            var query = _context.Banners.AsQueryable();

            if (!string.IsNullOrWhiteSpace(position))
                query = query.Where(b => b.Position == position);

            if (isActive.HasValue)
                query = query.Where(b => b.IsActive == isActive.Value);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(b => b.Title.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "title" => request.SortDescending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
                "sortorder" => request.SortDescending ? query.OrderByDescending(b => b.SortOrder) : query.OrderBy(b => b.SortOrder),
                _ => query.OrderBy(b => b.SortOrder)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<Banner?> GetByIdAsync(Guid id)
            => await _context.Banners.FirstOrDefaultAsync(b => b.Id == id);

        public async Task<IEnumerable<Banner>> GetActiveBannersAsync(string? position = null)
        {
            var now = DateTime.UtcNow;
            var query = _context.Banners
                .Where(b => b.IsActive && b.StartDate <= now && b.EndDate >= now);

            if (!string.IsNullOrWhiteSpace(position))
                query = query.Where(b => b.Position == position);

            return await query.OrderBy(b => b.SortOrder).ToListAsync();
        }

        public async Task<Banner> CreateAsync(Banner banner)
        {
            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();
            return banner;
        }

        public async Task UpdateAsync(Banner banner)
        {
            _context.Banners.Update(banner);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Banner banner)
        {
            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();
        }
    }
}
