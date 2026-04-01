using Application.DTOs.Brands;
using Application.DTOs.Common;
using Application.Helpers;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Brands
{
    public class BrandUseCase
    {
        private readonly IBrandRepository _brandRepository;

        public BrandUseCase(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<BrandDto>> GetPagedAsync(PagedRequest request)
        {
            var paged = await _brandRepository.GetPagedAsync(request);
            return new PagedResult<BrandDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<BrandDto?> GetByIdAsync(Guid id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            return brand == null ? null : MapToDto(brand);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<BrandDto> CreateAsync(CreateBrandRequest request)
        {
            var slug = SlugHelper.Generate(request.Name);

            if (await _brandRepository.SlugExistsAsync(slug))
                throw new InvalidOperationException($"A brand with slug '{slug}' already exists.");

            var brand = new Brand
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = slug,
                Description = request.Description,
                LogoUrl = request.LogoUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _brandRepository.CreateAsync(brand);
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateBrandRequest request)
        {
            var brand = await _brandRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Brand {id} not found.");

            if (request.Name != null)
            {
                brand.Name = request.Name;
                brand.Slug = SlugHelper.Generate(request.Name);
            }

            if (request.Description != null) brand.Description = request.Description;
            if (request.LogoUrl != null) brand.LogoUrl = request.LogoUrl;
            if (request.IsActive.HasValue) brand.IsActive = request.IsActive.Value;

            await _brandRepository.UpdateAsync(brand);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var brand = await _brandRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Brand {id} not found.");

            await _brandRepository.DeleteAsync(brand);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static BrandDto MapToDto(Brand b) => new()
        {
            Id = b.Id,
            Name = b.Name,
            Slug = b.Slug,
            Description = b.Description,
            LogoUrl = b.LogoUrl,
            IsActive = b.IsActive,
            CreatedAt = b.CreatedAt
        };
    }
}
