using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class ProductImage : BaseEntity, ITenantScoped
    {
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string ImageUrl { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? AltText { get; set; }

        public int SortOrder { get; set; }
        public bool IsPrimary { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public ProductVariant? ProductVariant { get; set; }
    }
}
