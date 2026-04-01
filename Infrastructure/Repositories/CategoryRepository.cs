using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
            => await _context.Categories.OrderBy(c => c.SortOrder).ThenBy(c => c.Name).ToListAsync();

        public async Task<PagedResult<Category>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                "sortorder" => request.SortDescending ? query.OrderByDescending(c => c.SortOrder) : query.OrderBy(c => c.SortOrder),
                _ => query.OrderBy(c => c.SortOrder)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<IEnumerable<Category>> GetTreeAsync()
            => await _context.Categories.Include(c => c.ParentCategory).ToListAsync();

        public async Task<Category?> GetByIdAsync(Guid id)
            => await _context.Categories.Include(c => c.ParentCategory).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<bool> SlugExistsAsync(string slug)
            => await _context.Categories.AnyAsync(c => c.Slug == slug);

        public async Task<bool> HasChildrenAsync(Guid id)
            => await _context.Categories.AnyAsync(c => c.ParentCategoryId == id);

        public async Task<bool> HasProductsAsync(Guid id)
            => await _context.Categories.Where(c => c.Id == id).AnyAsync(c => c.Products.Any());

        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
