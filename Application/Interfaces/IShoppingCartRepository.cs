using Domain.Entities;

namespace Application.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCart?> GetActiveByCustomerIdAsync(Guid customerId);
        Task<ShoppingCart?> GetActiveByGuestIdAsync(Guid guestId);
        Task<ShoppingCart?> GetByIdAsync(Guid id);
        Task<ShoppingCart> CreateAsync(ShoppingCart cart);
        Task UpdateAsync(ShoppingCart cart);
        Task<ShoppingCartItem> AddItemAsync(ShoppingCartItem item);
        Task UpdateItemAsync(ShoppingCartItem item);
        Task RemoveItemAsync(ShoppingCartItem item);
        Task<ShoppingCartItem?> GetItemByCartAndVariantAsync(Guid cartId, Guid variantId);
    }
}
