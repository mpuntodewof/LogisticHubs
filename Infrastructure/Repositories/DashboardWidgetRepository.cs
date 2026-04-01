using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DashboardWidgetRepository : IDashboardWidgetRepository
    {
        private readonly AppDbContext _context;

        public DashboardWidgetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<DashboardWidget>> GetPagedAsync(PagedRequest request, Guid? userId = null)
        {
            var query = _context.DashboardWidgets.AsQueryable();

            if (userId.HasValue)
                query = query.Where(e => e.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(e => e.Title.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "title" => request.SortDescending ? query.OrderByDescending(e => e.Title) : query.OrderBy(e => e.Title),
                _ => query.OrderBy(e => e.SortOrder)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<List<DashboardWidget>> GetByUserIdAsync(Guid? userId)
            => await _context.DashboardWidgets
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.SortOrder)
                .ToListAsync();

        public async Task<DashboardWidget?> GetByIdAsync(Guid id)
            => await _context.DashboardWidgets.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<DashboardWidget> CreateAsync(DashboardWidget entity)
        {
            _context.DashboardWidgets.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(DashboardWidget entity)
        {
            _context.DashboardWidgets.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(DashboardWidget entity)
        {
            _context.DashboardWidgets.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
