using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Products
{
    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public int SortOrder { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class CreateProductImageRequest
    {
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }

        [Required, StringLength(1000)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(255)]
        public string? AltText { get; set; }

        public int SortOrder { get; set; }
        public bool IsPrimary { get; set; }
    }
}
