using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Product : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required]
        [MaxLength(500)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public Guid? CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public Guid BaseUnitOfMeasureId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public Guid TenantId { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public Category? Category { get; set; }
        public Brand? Brand { get; set; }
        public UnitOfMeasure BaseUnitOfMeasure { get; set; } = null!;
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductTaxRate> ProductTaxRates { get; set; } = new List<ProductTaxRate>();
        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public ICollection<PromotionProduct> PromotionProducts { get; set; } = new List<PromotionProduct>();
    }
}
