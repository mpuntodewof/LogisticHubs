using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Ecommerce
{
    public class WishlistDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class WishlistDetailDto : WishlistDto
    {
        public IList<WishlistItemDto> Items { get; set; } = new List<WishlistItemDto>();
    }

    public class WishlistItemDto
    {
        public Guid Id { get; set; }
        public Guid WishlistId { get; set; }
        public Guid ProductVariantId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        public decimal SellingPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateWishlistRequest
    {
        [MaxLength(200)]
        public string? Name { get; set; }
    }

    public class AddWishlistItemRequest
    {
        [Required]
        public Guid ProductVariantId { get; set; }
    }
}
