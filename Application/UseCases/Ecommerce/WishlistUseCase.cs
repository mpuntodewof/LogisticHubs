using Application.DTOs.Ecommerce;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Ecommerce
{
    public class WishlistUseCase
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistUseCase(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<IEnumerable<WishlistDto>> GetByCustomerIdAsync(Guid customerId)
        {
            var wishlists = await _wishlistRepository.GetByCustomerIdAsync(customerId);
            return wishlists.Select(MapToDto);
        }

        public async Task<WishlistDetailDto?> GetByIdAsync(Guid id)
        {
            var wishlist = await _wishlistRepository.GetByIdAsync(id);
            return wishlist == null ? null : MapToDetailDto(wishlist);
        }

        public async Task<WishlistDto> CreateAsync(Guid customerId, CreateWishlistRequest request)
        {
            var wishlist = new Wishlist
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Name = request.Name ?? "My Wishlist",
                CreatedAt = DateTime.UtcNow
            };

            var created = await _wishlistRepository.CreateAsync(wishlist);
            return MapToDto(created);
        }

        public async Task DeleteAsync(Guid id)
        {
            var wishlist = await _wishlistRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Wishlist not found.");

            await _wishlistRepository.DeleteAsync(wishlist);
        }

        public async Task<WishlistDetailDto> AddItemAsync(Guid wishlistId, AddWishlistItemRequest request)
        {
            var wishlist = await _wishlistRepository.GetByIdAsync(wishlistId)
                ?? throw new InvalidOperationException("Wishlist not found.");

            var exists = await _wishlistRepository.ItemExistsAsync(wishlistId, request.ProductVariantId);
            if (exists)
                throw new InvalidOperationException("This product variant is already in the wishlist.");

            var item = new WishlistItem
            {
                Id = Guid.NewGuid(),
                WishlistId = wishlistId,
                ProductVariantId = request.ProductVariantId,
                CreatedAt = DateTime.UtcNow
            };

            await _wishlistRepository.AddItemAsync(item);

            var updated = await _wishlistRepository.GetByIdAsync(wishlistId);
            return MapToDetailDto(updated!);
        }

        public async Task<WishlistDetailDto> RemoveItemAsync(Guid wishlistId, Guid itemId)
        {
            var wishlist = await _wishlistRepository.GetByIdAsync(wishlistId)
                ?? throw new InvalidOperationException("Wishlist not found.");

            var item = wishlist.Items.FirstOrDefault(i => i.Id == itemId)
                ?? throw new InvalidOperationException("Wishlist item not found.");

            await _wishlistRepository.RemoveItemAsync(item);

            var updated = await _wishlistRepository.GetByIdAsync(wishlistId);
            return MapToDetailDto(updated!);
        }

        private static WishlistDto MapToDto(Wishlist w) => new()
        {
            Id = w.Id,
            CustomerId = w.CustomerId,
            Name = w.Name,
            CreatedAt = w.CreatedAt
        };

        private static WishlistDetailDto MapToDetailDto(Wishlist w) => new()
        {
            Id = w.Id,
            CustomerId = w.CustomerId,
            Name = w.Name,
            CreatedAt = w.CreatedAt,
            Items = w.Items?.Select(MapItemToDto).ToList() ?? new List<WishlistItemDto>()
        };

        private static WishlistItemDto MapItemToDto(WishlistItem i) => new()
        {
            Id = i.Id,
            WishlistId = i.WishlistId,
            ProductVariantId = i.ProductVariantId,
            Sku = i.ProductVariant?.Sku ?? string.Empty,
            ProductName = i.ProductVariant?.Product?.Name ?? string.Empty,
            VariantName = i.ProductVariant?.Name ?? string.Empty,
            SellingPrice = i.ProductVariant?.SellingPrice ?? 0m,
            CreatedAt = i.CreatedAt
        };
    }
}
