using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Ecommerce
{
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? GuestId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ShoppingCartDetailDto : ShoppingCartDto
    {
        public IList<ShoppingCartItemDto> Items { get; set; } = new List<ShoppingCartItemDto>();
    }

    public class ShoppingCartItemDto
    {
        public Guid Id { get; set; }
        public Guid ShoppingCartId { get; set; }
        public Guid ProductVariantId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }

    public class AddCartItemRequest
    {
        [Required]
        public Guid ProductVariantId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }

    public class UpdateCartItemRequest
    {
        [Required]
        public int Quantity { get; set; }
    }

    public class MergeCartRequest
    {
        [Required]
        public Guid GuestId { get; set; }
    }
}
