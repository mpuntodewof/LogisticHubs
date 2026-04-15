using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SalesChannelRepository : ISalesChannelRepository
    {
        private readonly AppDbContext _context;

        public SalesChannelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<SalesChannel>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.Set<SalesChannel>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(search));
            }

            query = query.OrderBy(c => c.Name);
            return await query.ToPagedResultAsync(request);
        }

        public async Task<SalesChannel?> GetByIdAsync(Guid id)
            => await _context.Set<SalesChannel>().FirstOrDefaultAsync(c => c.Id == id);

        public async Task<SalesChannel?> GetBySlugAsync(string slug)
            => await _context.Set<SalesChannel>().FirstOrDefaultAsync(c => c.Slug == slug);

        public async Task<SalesChannel> CreateAsync(SalesChannel channel)
        {
            _context.Set<SalesChannel>().Add(channel);
            return channel;
        }

        public async Task UpdateAsync(SalesChannel channel)
        {
            // Tracked by change tracker
        }

        public async Task DeleteAsync(SalesChannel channel)
        {
            _context.Set<SalesChannel>().Remove(channel);
        }
    }
}
