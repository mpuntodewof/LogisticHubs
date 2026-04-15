using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Product>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.Set<Product>()
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.BaseUnitOfMeasure)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(search) || p.Slug.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "status" => request.SortDescending ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<Product?> GetByIdAsync(Guid id)
            => await _context.Set<Product>()
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.BaseUnitOfMeasure)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Product?> GetDetailByIdAsync(Guid id)
            => await _context.Set<Product>()
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.BaseUnitOfMeasure)
                .Include(p => p.Variants)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<bool> SlugExistsAsync(string slug)
            => await _context.Set<Product>().AnyAsync(p => p.Slug == slug);

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Set<Product>().Add(product);
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Set<Product>().Update(product);
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Set<Product>().Remove(product);
        }
    }
}
