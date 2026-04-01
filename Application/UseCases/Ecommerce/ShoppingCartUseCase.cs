using Application.DTOs.Ecommerce;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Ecommerce
{
    public class ShoppingCartUseCase
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IProductVariantRepository _productVariantRepository;

        public ShoppingCartUseCase(
            IShoppingCartRepository cartRepository,
            IProductVariantRepository productVariantRepository)
        {
            _cartRepository = cartRepository;
            _productVariantRepository = productVariantRepository;
        }

        public async Task<ShoppingCartDetailDto> GetOrCreateCartAsync(Guid? customerId, Guid? guestId)
        {
            ShoppingCart? cart = null;

            if (customerId.HasValue)
                cart = await _cartRepository.GetActiveByCustomerIdAsync(customerId.Value);
            else if (guestId.HasValue)
                cart = await _cartRepository.GetActiveByGuestIdAsync(guestId.Value);

            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    GuestId = guestId ?? Guid.NewGuid(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                cart = await _cartRepository.CreateAsync(cart);
            }

            return MapToDetailDto(cart);
        }

        public async Task<ShoppingCartDetailDto?> GetByIdAsync(Guid id)
        {
            var cart = await _cartRepository.GetByIdAsync(id);
            return cart == null ? null : MapToDetailDto(cart);
        }

        public async Task<ShoppingCartDetailDto> AddItemAsync(Guid cartId, AddCartItemRequest request)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId)
                ?? throw new InvalidOperationException("Shopping cart not found.");

            var variant = await _productVariantRepository.GetByIdAsync(request.ProductVariantId)
                ?? throw new InvalidOperationException($"Product variant with ID '{request.ProductVariantId}' not found.");

            // Upsert: if variant already in cart, add quantity
            var existingItem = await _cartRepository.GetItemByCartAndVariantAsync(cartId, request.ProductVariantId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                existingItem.UnitPrice = variant.SellingPrice;
                existingItem.UpdatedAt = DateTime.UtcNow;
                await _cartRepository.UpdateItemAsync(existingItem);
            }
            else
            {
                var item = new ShoppingCartItem
                {
                    Id = Guid.NewGuid(),
                    ShoppingCartId = cartId,
                    ProductVariantId = variant.Id,
                    Quantity = request.Quantity,
                    UnitPrice = variant.SellingPrice,
                    CreatedAt = DateTime.UtcNow
                };

                await _cartRepository.AddItemAsync(item);
            }

            // Re-fetch to get updated items with navigation
            var updatedCart = await _cartRepository.GetByIdAsync(cartId);
            return MapToDetailDto(updatedCart!);
        }

        public async Task<ShoppingCartDetailDto> UpdateItemAsync(Guid cartId, Guid itemId, UpdateCartItemRequest request)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId)
                ?? throw new InvalidOperationException("Shopping cart not found.");

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId)
                ?? throw new InvalidOperationException("Cart item not found.");

            if (request.Quantity <= 0)
            {
                await _cartRepository.RemoveItemAsync(item);
            }
            else
            {
                item.Quantity = request.Quantity;
                item.UpdatedAt = DateTime.UtcNow;
                await _cartRepository.UpdateItemAsync(item);
            }

            var updatedCart = await _cartRepository.GetByIdAsync(cartId);
            return MapToDetailDto(updatedCart!);
        }

        public async Task<ShoppingCartDetailDto> RemoveItemAsync(Guid cartId, Guid itemId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId)
                ?? throw new InvalidOperationException("Shopping cart not found.");

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId)
                ?? throw new InvalidOperationException("Cart item not found.");

            await _cartRepository.RemoveItemAsync(item);

            var updatedCart = await _cartRepository.GetByIdAsync(cartId);
            return MapToDetailDto(updatedCart!);
        }

        public async Task MergeGuestCartAsync(Guid customerId, Guid guestId)
        {
            var guestCart = await _cartRepository.GetActiveByGuestIdAsync(guestId);
            if (guestCart == null) return;

            var customerCart = await _cartRepository.GetActiveByCustomerIdAsync(customerId);

            if (customerCart == null)
            {
                // Just assign the guest cart to the customer
                guestCart.CustomerId = customerId;
                guestCart.GuestId = null;
                guestCart.UpdatedAt = DateTime.UtcNow;
                await _cartRepository.UpdateAsync(guestCart);
                return;
            }

            // Merge items from guest cart into customer cart
            foreach (var guestItem in guestCart.Items)
            {
                var existingItem = await _cartRepository.GetItemByCartAndVariantAsync(customerCart.Id, guestItem.ProductVariantId);

                if (existingItem != null)
                {
                    existingItem.Quantity += guestItem.Quantity;
                    existingItem.UpdatedAt = DateTime.UtcNow;
                    await _cartRepository.UpdateItemAsync(existingItem);
                }
                else
                {
                    var newItem = new ShoppingCartItem
                    {
                        Id = Guid.NewGuid(),
                        ShoppingCartId = customerCart.Id,
                        ProductVariantId = guestItem.ProductVariantId,
                        Quantity = guestItem.Quantity,
                        UnitPrice = guestItem.UnitPrice,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _cartRepository.AddItemAsync(newItem);
                }
            }

            // Deactivate guest cart
            guestCart.IsActive = false;
            guestCart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateAsync(guestCart);
        }

        public async Task DeleteAsync(Guid id)
        {
            var cart = await _cartRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Shopping cart not found.");

            cart.IsActive = false;
            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateAsync(cart);
        }

        private static ShoppingCartDto MapToDto(ShoppingCart c) => new()
        {
            Id = c.Id,
            CustomerId = c.CustomerId,
            GuestId = c.GuestId,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        };

        private static ShoppingCartDetailDto MapToDetailDto(ShoppingCart c) => new()
        {
            Id = c.Id,
            CustomerId = c.CustomerId,
            GuestId = c.GuestId,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            Items = c.Items?.Select(MapItemToDto).ToList() ?? new List<ShoppingCartItemDto>()
        };

        private static ShoppingCartItemDto MapItemToDto(ShoppingCartItem i) => new()
        {
            Id = i.Id,
            ShoppingCartId = i.ShoppingCartId,
            ProductVariantId = i.ProductVariantId,
            Sku = i.ProductVariant?.Sku ?? string.Empty,
            ProductName = i.ProductVariant?.Product?.Name ?? string.Empty,
            VariantName = i.ProductVariant?.Name ?? string.Empty,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        };
    }
}
