using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly AppDbContext _context;

        public ShoppingCartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCart?> GetActiveByCustomerIdAsync(Guid customerId)
            => await _context.ShoppingCarts
                .Include(c => c.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive);

        public async Task<ShoppingCart?> GetActiveByGuestIdAsync(Guid guestId)
            => await _context.ShoppingCarts
                .Include(c => c.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(c => c.GuestId == guestId && c.IsActive);

        public async Task<ShoppingCart?> GetByIdAsync(Guid id)
            => await _context.ShoppingCarts
                .Include(c => c.Items)
                    .ThenInclude(i => i.ProductVariant)
                        .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<ShoppingCart> CreateAsync(ShoppingCart cart)
        {
            _context.ShoppingCarts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task UpdateAsync(ShoppingCart cart)
        {
            _context.ShoppingCarts.Update(cart);
            await _context.SaveChangesAsync();
        }

        public async Task<ShoppingCartItem> AddItemAsync(ShoppingCartItem item)
        {
            _context.ShoppingCartItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task UpdateItemAsync(ShoppingCartItem item)
        {
            _context.ShoppingCartItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(ShoppingCartItem item)
        {
            _context.ShoppingCartItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<ShoppingCartItem?> GetItemByCartAndVariantAsync(Guid cartId, Guid variantId)
            => await _context.ShoppingCartItems
                .FirstOrDefaultAsync(i => i.ShoppingCartId == cartId && i.ProductVariantId == variantId);
    }
}
