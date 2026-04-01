using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Products
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public Guid? BrandId { get; set; }
        public string? BrandName { get; set; }
        public Guid BaseUnitOfMeasureId { get; set; }
        public string BaseUnitAbbreviation { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int VariantCount { get; set; }
    }

    public class ProductDetailDto : ProductDto
    {
        public IList<ProductVariantDto> Variants { get; set; } = new List<ProductVariantDto>();
        public IList<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
    }

    public class CreateProductRequest
    {
        [Required, StringLength(500)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        public Guid? CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public Guid BaseUnitOfMeasureId { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Draft;
    }

    public class UpdateProductRequest
    {
        [StringLength(500)]
        public string? Name { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public Guid? CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public Guid? BaseUnitOfMeasureId { get; set; }
        public ProductStatus? Status { get; set; }
        public bool? IsActive { get; set; }
    }
}
