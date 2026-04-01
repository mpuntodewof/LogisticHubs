using Application.DTOs.Common;
using Application.DTOs.Products;
using Application.Helpers;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Products
{
    public class ProductUseCase
    {
        private readonly IProductRepository _productRepository;

        public ProductUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedResult<ProductDto>> GetPagedAsync(PagedRequest request)
        {
            var result = await _productRepository.GetPagedAsync(request);

            return new PagedResult<ProductDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<ProductDetailDto?> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetDetailByIdAsync(id);
            return product == null ? null : MapToDetailDto(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductRequest request)
        {
            var slug = SlugHelper.Generate(request.Name);

            if (await _productRepository.SlugExistsAsync(slug))
                throw new InvalidOperationException($"A product with slug '{slug}' already exists.");

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = slug,
                Description = request.Description,
                CategoryId = request.CategoryId,
                BrandId = request.BrandId,
                BaseUnitOfMeasureId = request.BaseUnitOfMeasureId,
                Status = request.Status.ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _productRepository.CreateAsync(product);
            return MapToDto(created);
        }

        public async Task UpdateAsync(Guid id, UpdateProductRequest request)
        {
            var product = await _productRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Product not found.");

            if (request.Name != null)
            {
                product.Name = request.Name;
                product.Slug = SlugHelper.Generate(request.Name);
            }

            if (request.Description != null) product.Description = request.Description;
            if (request.CategoryId.HasValue) product.CategoryId = request.CategoryId;
            if (request.BrandId.HasValue) product.BrandId = request.BrandId;
            if (request.BaseUnitOfMeasureId.HasValue) product.BaseUnitOfMeasureId = request.BaseUnitOfMeasureId.Value;
            if (request.Status.HasValue) product.Status = request.Status.Value.ToString();
            if (request.IsActive.HasValue) product.IsActive = request.IsActive.Value;

            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Product not found.");

            await _productRepository.DeleteAsync(product);
        }

        private static ProductDto MapToDto(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            Description = p.Description,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            BrandId = p.BrandId,
            BrandName = p.Brand?.Name,
            BaseUnitOfMeasureId = p.BaseUnitOfMeasureId,
            BaseUnitAbbreviation = p.BaseUnitOfMeasure?.Abbreviation ?? string.Empty,
            Status = p.Status,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            VariantCount = p.Variants?.Count ?? 0
        };

        private static ProductDetailDto MapToDetailDto(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            Description = p.Description,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            BrandId = p.BrandId,
            BrandName = p.Brand?.Name,
            BaseUnitOfMeasureId = p.BaseUnitOfMeasureId,
            BaseUnitAbbreviation = p.BaseUnitOfMeasure?.Abbreviation ?? string.Empty,
            Status = p.Status,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            VariantCount = p.Variants?.Count ?? 0,
            Variants = p.Variants?.Select(MapVariantToDto).ToList() ?? new List<ProductVariantDto>(),
            Images = p.Images?.Select(MapImageToDto).ToList() ?? new List<ProductImageDto>()
        };

        private static ProductVariantDto MapVariantToDto(ProductVariant v) => new()
        {
            Id = v.Id,
            ProductId = v.ProductId,
            Sku = v.Sku,
            Name = v.Name,
            VariantAttributes = v.VariantAttributes,
            Barcode = v.Barcode,
            CostPrice = v.CostPrice,
            SellingPrice = v.SellingPrice,
            Weight = v.Weight,
            IsActive = v.IsActive,
            CreatedAt = v.CreatedAt
        };

        private static ProductImageDto MapImageToDto(ProductImage img) => new()
        {
            Id = img.Id,
            ProductId = img.ProductId,
            ProductVariantId = img.ProductVariantId,
            ImageUrl = img.ImageUrl,
            AltText = img.AltText,
            SortOrder = img.SortOrder,
            IsPrimary = img.IsPrimary
        };
    }
}
