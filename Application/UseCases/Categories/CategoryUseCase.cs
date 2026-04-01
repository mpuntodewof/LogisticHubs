using Application.DTOs.Categories;
using Application.DTOs.Common;
using Application.Helpers;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Categories
{
    public class CategoryUseCase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryUseCase(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<CategoryDto>> GetPagedAsync(PagedRequest request)
        {
            var paged = await _categoryRepository.GetPagedAsync(request);
            return new PagedResult<CategoryDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<IEnumerable<CategoryTreeDto>> GetTreeAsync()
        {
            var all = await _categoryRepository.GetAllAsync();
            var list = all.ToList();

            var lookup = list
                .GroupBy(c => c.ParentCategoryId)
                .ToDictionary(g => g.Key, g => g.OrderBy(c => c.SortOrder).ToList());

            return BuildTree(lookup, null);
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category == null ? null : MapToDto(category);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
        {
            var slug = SlugHelper.Generate(request.Name);

            if (request.ParentCategoryId.HasValue)
            {
                _ = await _categoryRepository.GetByIdAsync(request.ParentCategoryId.Value)
                    ?? throw new KeyNotFoundException($"Parent category {request.ParentCategoryId.Value} not found.");
            }

            if (await _categoryRepository.SlugExistsAsync(slug))
                throw new InvalidOperationException($"A category with slug '{slug}' already exists.");

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = slug,
                Description = request.Description,
                ParentCategoryId = request.ParentCategoryId,
                SortOrder = request.SortOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _categoryRepository.CreateAsync(category);
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateCategoryRequest request)
        {
            var category = await _categoryRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Category {id} not found.");

            if (request.Name != null)
            {
                category.Name = request.Name;
                category.Slug = SlugHelper.Generate(request.Name);
            }

            if (request.Description != null) category.Description = request.Description;
            if (request.ParentCategoryId.HasValue) category.ParentCategoryId = request.ParentCategoryId;
            if (request.SortOrder.HasValue) category.SortOrder = request.SortOrder.Value;
            if (request.IsActive.HasValue) category.IsActive = request.IsActive.Value;

            await _categoryRepository.UpdateAsync(category);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Category {id} not found.");

            if (await _categoryRepository.HasChildrenAsync(id))
                throw new InvalidOperationException("Cannot delete a category that has child categories.");

            if (await _categoryRepository.HasProductsAsync(id))
                throw new InvalidOperationException("Cannot delete a category that has associated products.");

            await _categoryRepository.DeleteAsync(category);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static IList<CategoryTreeDto> BuildTree(
            Dictionary<Guid?, List<Category>> lookup, Guid? parentId)
        {
            if (!lookup.TryGetValue(parentId, out var children))
                return new List<CategoryTreeDto>();

            return children.Select(c => new CategoryTreeDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                SortOrder = c.SortOrder,
                Children = BuildTree(lookup, c.Id)
            }).ToList();
        }

        private static CategoryDto MapToDto(Category c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            Description = c.Description,
            ParentCategoryId = c.ParentCategoryId,
            ParentCategoryName = c.ParentCategory?.Name,
            SortOrder = c.SortOrder,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        };
    }
}
