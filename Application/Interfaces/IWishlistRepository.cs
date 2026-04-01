using Domain.Entities;

namespace Application.Interfaces
{
    public interface IWishlistRepository
    {
        Task<IEnumerable<Wishlist>> GetByCustomerIdAsync(Guid customerId);
        Task<Wishlist?> GetByIdAsync(Guid id);
        Task<Wishlist> CreateAsync(Wishlist wishlist);
        Task DeleteAsync(Wishlist wishlist);
        Task<WishlistItem> AddItemAsync(WishlistItem item);
        Task RemoveItemAsync(WishlistItem item);
        Task<bool> ItemExistsAsync(Guid wishlistId, Guid variantId);
    }
}
