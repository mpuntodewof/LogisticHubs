using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PageRepository : IPageRepository
    {
        private readonly AppDbContext _context;

        public PageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Page>> GetPagedAsync(PagedRequest request, string? status = null)
        {
            var query = _context.Pages.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(p => p.Status == status);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(search) || p.Slug.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "title" => request.SortDescending ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
                "slug" => request.SortDescending ? query.OrderByDescending(p => p.Slug) : query.OrderBy(p => p.Slug),
                _ => query.OrderBy(p => p.SortOrder)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<Page?> GetByIdAsync(Guid id)
            => await _context.Pages.FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Page?> GetBySlugAsync(string slug)
            => await _context.Pages.FirstOrDefaultAsync(p => p.Slug == slug);

        public async Task<bool> SlugExistsAsync(string slug)
            => await _context.Pages.AnyAsync(p => p.Slug == slug);

        public async Task<Page> CreateAsync(Page page)
        {
            _context.Pages.Add(page);
            await _context.SaveChangesAsync();
            return page;
        }

        public async Task UpdateAsync(Page page)
        {
            _context.Pages.Update(page);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Page page)
        {
            _context.Pages.Remove(page);
            await _context.SaveChangesAsync();
        }
    }
}
