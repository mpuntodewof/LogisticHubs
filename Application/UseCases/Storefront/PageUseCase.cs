using Application.DTOs.Common;
using Application.DTOs.Storefront;
using Application.Helpers;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Storefront
{
    public class PageUseCase
    {
        private readonly IPageRepository _pageRepository;

        public PageUseCase(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public async Task<PagedResult<PageDto>> GetPagedAsync(PagedRequest request, string? status = null)
        {
            var paged = await _pageRepository.GetPagedAsync(request, status);
            return new PagedResult<PageDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<PageDetailDto?> GetByIdAsync(Guid id)
        {
            var page = await _pageRepository.GetByIdAsync(id);
            return page == null ? null : MapToDetailDto(page);
        }

        public async Task<PageDetailDto?> GetBySlugAsync(string slug)
        {
            var page = await _pageRepository.GetBySlugAsync(slug);
            return page == null ? null : MapToDetailDto(page);
        }

        public async Task<PageDto> CreateAsync(CreatePageRequest request)
        {
            var slug = SlugHelper.Generate(request.Title);

            if (await _pageRepository.SlugExistsAsync(slug))
                throw new InvalidOperationException($"A page with slug '{slug}' already exists.");

            var page = new Page
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Slug = slug,
                Content = request.Content,
                MetaTitle = request.MetaTitle,
                MetaDescription = request.MetaDescription,
                Status = request.Status.ToString(),
                SortOrder = request.SortOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            if (request.Status == PageStatus.Published)
                page.PublishedAt = DateTime.UtcNow;

            var created = await _pageRepository.CreateAsync(page);
            return MapToDto(created);
        }

        public async Task UpdateAsync(Guid id, UpdatePageRequest request)
        {
            var page = await _pageRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Page {id} not found.");

            if (request.Title != null)
            {
                page.Title = request.Title;
                page.Slug = SlugHelper.Generate(request.Title);
            }

            if (request.Content != null) page.Content = request.Content;
            if (request.MetaTitle != null) page.MetaTitle = request.MetaTitle;
            if (request.MetaDescription != null) page.MetaDescription = request.MetaDescription;
            if (request.SortOrder.HasValue) page.SortOrder = request.SortOrder.Value;
            if (request.IsActive.HasValue) page.IsActive = request.IsActive.Value;

            if (request.Status.HasValue)
            {
                page.Status = request.Status.Value.ToString();
                if (request.Status.Value == PageStatus.Published && page.PublishedAt == null)
                    page.PublishedAt = DateTime.UtcNow;
            }

            page.UpdatedAt = DateTime.UtcNow;

            await _pageRepository.UpdateAsync(page);
        }

        public async Task DeleteAsync(Guid id)
        {
            var page = await _pageRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Page {id} not found.");

            await _pageRepository.DeleteAsync(page);
        }

        private static PageDto MapToDto(Page p) => new()
        {
            Id = p.Id,
            Title = p.Title,
            Slug = p.Slug,
            MetaTitle = p.MetaTitle,
            MetaDescription = p.MetaDescription,
            Status = p.Status,
            SortOrder = p.SortOrder,
            IsActive = p.IsActive,
            PublishedAt = p.PublishedAt,
            CreatedAt = p.CreatedAt
        };

        private static PageDetailDto MapToDetailDto(Page p) => new()
        {
            Id = p.Id,
            Title = p.Title,
            Slug = p.Slug,
            Content = p.Content,
            MetaTitle = p.MetaTitle,
            MetaDescription = p.MetaDescription,
            Status = p.Status,
            SortOrder = p.SortOrder,
            IsActive = p.IsActive,
            PublishedAt = p.PublishedAt,
            CreatedAt = p.CreatedAt
        };
    }
}
