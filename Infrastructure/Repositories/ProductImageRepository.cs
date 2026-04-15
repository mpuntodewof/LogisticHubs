using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext _context;

        public ProductImageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(Guid productId)
            => await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .OrderBy(pi => pi.SortOrder)
                .ToListAsync();

        public async Task<ProductImage?> GetByIdAsync(Guid id)
            => await _context.ProductImages.FindAsync(id);

        public async Task<ProductImage> CreateAsync(ProductImage image)
        {
            _context.ProductImages.Add(image);
            return image;
        }

        public async Task DeleteAsync(ProductImage image)
        {
            _context.ProductImages.Remove(image);
        }
    }
}
