using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class StorefrontConfig : BaseEntity, ITenantScoped
    {
        [Required]
        [MaxLength(200)]
        public string StoreName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? LogoUrl { get; set; }

        [MaxLength(1000)]
        public string? FaviconUrl { get; set; }

        [MaxLength(20)]
        public string? PrimaryColor { get; set; }

        [MaxLength(20)]
        public string? SecondaryColor { get; set; }

        [MaxLength(20)]
        public string? AccentColor { get; set; }

        [MaxLength(200)]
        public string? MetaTitle { get; set; }

        [MaxLength(500)]
        public string? MetaDescription { get; set; }

        [MaxLength(500)]
        public string? MetaKeywords { get; set; }

        [MaxLength(500)]
        public string? FacebookUrl { get; set; }

        [MaxLength(500)]
        public string? InstagramUrl { get; set; }

        [MaxLength(500)]
        public string? TwitterUrl { get; set; }

        [MaxLength(50)]
        public string? WhatsAppNumber { get; set; }

        [MaxLength(500)]
        public string? TokopediaUrl { get; set; }

        [MaxLength(500)]
        public string? ShopeeUrl { get; set; }

        [MaxLength(8000)]
        public string? CustomCss { get; set; }

        [MaxLength(8000)]
        public string? CustomJs { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
    }
}
