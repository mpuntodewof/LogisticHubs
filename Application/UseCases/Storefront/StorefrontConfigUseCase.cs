using Application.DTOs.Storefront;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Storefront
{
    public class StorefrontConfigUseCase
    {
        private readonly IStorefrontConfigRepository _repository;

        public StorefrontConfigUseCase(IStorefrontConfigRepository repository)
        {
            _repository = repository;
        }

        public async Task<StorefrontConfigDto> GetAsync()
        {
            var config = await _repository.GetByTenantAsync();

            if (config == null)
            {
                config = new StorefrontConfig
                {
                    Id = Guid.NewGuid(),
                    StoreName = "My Store",
                    CreatedAt = DateTime.UtcNow
                };
                config = await _repository.CreateAsync(config);
            }

            return MapToDto(config);
        }

        public async Task UpdateAsync(UpdateStorefrontConfigRequest request)
        {
            var config = await _repository.GetByTenantAsync()
                ?? throw new KeyNotFoundException("Storefront config not found.");

            if (request.StoreName != null) config.StoreName = request.StoreName;
            if (request.LogoUrl != null) config.LogoUrl = request.LogoUrl;
            if (request.FaviconUrl != null) config.FaviconUrl = request.FaviconUrl;
            if (request.PrimaryColor != null) config.PrimaryColor = request.PrimaryColor;
            if (request.SecondaryColor != null) config.SecondaryColor = request.SecondaryColor;
            if (request.AccentColor != null) config.AccentColor = request.AccentColor;
            if (request.MetaTitle != null) config.MetaTitle = request.MetaTitle;
            if (request.MetaDescription != null) config.MetaDescription = request.MetaDescription;
            if (request.MetaKeywords != null) config.MetaKeywords = request.MetaKeywords;
            if (request.FacebookUrl != null) config.FacebookUrl = request.FacebookUrl;
            if (request.InstagramUrl != null) config.InstagramUrl = request.InstagramUrl;
            if (request.TwitterUrl != null) config.TwitterUrl = request.TwitterUrl;
            if (request.WhatsAppNumber != null) config.WhatsAppNumber = request.WhatsAppNumber;
            if (request.TokopediaUrl != null) config.TokopediaUrl = request.TokopediaUrl;
            if (request.ShopeeUrl != null) config.ShopeeUrl = request.ShopeeUrl;
            if (request.CustomCss != null) config.CustomCss = request.CustomCss;
            if (request.CustomJs != null) config.CustomJs = request.CustomJs;

            config.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(config);
        }

        private static StorefrontConfigDto MapToDto(StorefrontConfig c) => new()
        {
            Id = c.Id,
            StoreName = c.StoreName,
            LogoUrl = c.LogoUrl,
            FaviconUrl = c.FaviconUrl,
            PrimaryColor = c.PrimaryColor,
            SecondaryColor = c.SecondaryColor,
            AccentColor = c.AccentColor,
            MetaTitle = c.MetaTitle,
            MetaDescription = c.MetaDescription,
            MetaKeywords = c.MetaKeywords,
            FacebookUrl = c.FacebookUrl,
            InstagramUrl = c.InstagramUrl,
            TwitterUrl = c.TwitterUrl,
            WhatsAppNumber = c.WhatsAppNumber,
            TokopediaUrl = c.TokopediaUrl,
            ShopeeUrl = c.ShopeeUrl,
            CustomCss = c.CustomCss,
            CustomJs = c.CustomJs,
            CreatedAt = c.CreatedAt
        };
    }
}
