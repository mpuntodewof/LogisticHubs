using Application.DTOs.Common;
using Application.DTOs.Storefront;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Storefront
{
    public class BannerUseCase
    {
        private readonly IBannerRepository _bannerRepository;

        public BannerUseCase(IBannerRepository bannerRepository)
        {
            _bannerRepository = bannerRepository;
        }

        public async Task<PagedResult<BannerDto>> GetPagedAsync(PagedRequest request, string? position = null, bool? isActive = null)
        {
            var paged = await _bannerRepository.GetPagedAsync(request, position, isActive);
            return new PagedResult<BannerDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<BannerDto?> GetByIdAsync(Guid id)
        {
            var banner = await _bannerRepository.GetByIdAsync(id);
            return banner == null ? null : MapToDto(banner);
        }

        public async Task<IEnumerable<BannerDto>> GetActiveBannersAsync(string? position = null)
        {
            var banners = await _bannerRepository.GetActiveBannersAsync(position);
            return banners.Select(MapToDto);
        }

        public async Task<BannerDto> CreateAsync(CreateBannerRequest request)
        {
            var banner = new Banner
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                ImageUrl = request.ImageUrl,
                LinkUrl = request.LinkUrl,
                Position = request.Position.ToString(),
                SortOrder = request.SortOrder,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _bannerRepository.CreateAsync(banner);
            return MapToDto(created);
        }

        public async Task UpdateAsync(Guid id, UpdateBannerRequest request)
        {
            var banner = await _bannerRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Banner {id} not found.");

            if (request.Title != null) banner.Title = request.Title;
            if (request.ImageUrl != null) banner.ImageUrl = request.ImageUrl;
            if (request.LinkUrl != null) banner.LinkUrl = request.LinkUrl;
            if (request.Position.HasValue) banner.Position = request.Position.Value.ToString();
            if (request.SortOrder.HasValue) banner.SortOrder = request.SortOrder.Value;
            if (request.StartDate.HasValue) banner.StartDate = request.StartDate.Value;
            if (request.EndDate.HasValue) banner.EndDate = request.EndDate.Value;
            if (request.IsActive.HasValue) banner.IsActive = request.IsActive.Value;

            banner.UpdatedAt = DateTime.UtcNow;

            await _bannerRepository.UpdateAsync(banner);
        }

        public async Task DeleteAsync(Guid id)
        {
            var banner = await _bannerRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Banner {id} not found.");

            await _bannerRepository.DeleteAsync(banner);
        }

        private static BannerDto MapToDto(Banner b) => new()
        {
            Id = b.Id,
            Title = b.Title,
            ImageUrl = b.ImageUrl,
            LinkUrl = b.LinkUrl,
            Position = b.Position,
            SortOrder = b.SortOrder,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            IsActive = b.IsActive,
            CreatedAt = b.CreatedAt
        };
    }
}
