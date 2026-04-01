using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Products
{
    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? VariantAttributes { get; set; }
        public string? Barcode { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal? Weight { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateProductVariantRequest
    {
        public Guid ProductId { get; set; }

        [Required, StringLength(100)]
        public string Sku { get; set; } = string.Empty;

        [Required, StringLength(500)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? VariantAttributes { get; set; }

        [StringLength(100)]
        public string? Barcode { get; set; }

        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal? Weight { get; set; }
    }

    public class UpdateProductVariantRequest
    {
        [StringLength(100)]
        public string? Sku { get; set; }

        [StringLength(500)]
        public string? Name { get; set; }

        [StringLength(2000)]
        public string? VariantAttributes { get; set; }

        [StringLength(100)]
        public string? Barcode { get; set; }

        public decimal? CostPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? Weight { get; set; }
        public bool? IsActive { get; set; }
    }
}
