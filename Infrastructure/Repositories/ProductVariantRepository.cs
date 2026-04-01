using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly AppDbContext _context;

        public ProductVariantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ProductVariant>> GetPagedAsync(PagedRequest request, Guid? productId = null)
        {
            var query = _context.Set<ProductVariant>()
                .Include(v => v.Product)
                .AsQueryable();

            if (productId.HasValue)
            {
                query = query.Where(v => v.ProductId == productId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(v => v.Sku.ToLower().Contains(search) || v.Name.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "sku" => request.SortDescending ? query.OrderByDescending(v => v.Sku) : query.OrderBy(v => v.Sku),
                "name" => request.SortDescending ? query.OrderByDescending(v => v.Name) : query.OrderBy(v => v.Name),
                _ => query.OrderByDescending(v => v.CreatedAt)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<IEnumerable<ProductVariant>> GetByProductIdAsync(Guid productId)
            => await _context.Set<ProductVariant>()
                .Where(v => v.ProductId == productId)
                .OrderBy(v => v.Name)
                .ToListAsync();

        public async Task<ProductVariant?> GetByIdAsync(Guid id)
            => await _context.Set<ProductVariant>()
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.Id == id);

        public async Task<bool> SkuExistsAsync(string sku)
            => await _context.Set<ProductVariant>().AnyAsync(v => v.Sku == sku);

        public async Task<bool> BarcodeExistsAsync(string barcode)
            => await _context.Set<ProductVariant>().AnyAsync(v => v.Barcode != null && v.Barcode == barcode);

        public async Task<ProductVariant> CreateAsync(ProductVariant variant)
        {
            _context.Set<ProductVariant>().Add(variant);
            await _context.SaveChangesAsync();
            return variant;
        }

        public async Task UpdateAsync(ProductVariant variant)
        {
            _context.Set<ProductVariant>().Update(variant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductVariant variant)
        {
            _context.Set<ProductVariant>().Remove(variant);
            await _context.SaveChangesAsync();
        }
    }
}
