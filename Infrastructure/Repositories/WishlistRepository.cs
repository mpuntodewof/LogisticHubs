using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly AppDbContext _context;

        public WishlistRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Wishlist>> GetByCustomerIdAsync(Guid customerId)
            => await _context.Wishlists
                .Where(w => w.CustomerId == customerId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

        public async Task<Wishlist?> GetByIdAsync(Guid id)
            => await _context.Wishlists
                .Include(w => w.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(w => w.Id == id);

        public async Task<Wishlist> CreateAsync(Wishlist wishlist)
        {
            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();
            return wishlist;
        }

        public async Task DeleteAsync(Wishlist wishlist)
        {
            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task<WishlistItem> AddItemAsync(WishlistItem item)
        {
            _context.WishlistItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task RemoveItemAsync(WishlistItem item)
        {
            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ItemExistsAsync(Guid wishlistId, Guid variantId)
            => await _context.WishlistItems
                .AnyAsync(i => i.WishlistId == wishlistId && i.ProductVariantId == variantId);
    }
}
