namespace Application.DTOs.Storefront
{
    public class StorefrontConfigDto
    {
        public Guid Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? FaviconUrl { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? AccentColor { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? TokopediaUrl { get; set; }
        public string? ShopeeUrl { get; set; }
        public string? CustomCss { get; set; }
        public string? CustomJs { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateStorefrontConfigRequest
    {
        public string? StoreName { get; set; }
        public string? LogoUrl { get; set; }
        public string? FaviconUrl { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? AccentColor { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public string? FacebookUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? TokopediaUrl { get; set; }
        public string? ShopeeUrl { get; set; }
        public string? CustomCss { get; set; }
        public string? CustomJs { get; set; }
    }
}
