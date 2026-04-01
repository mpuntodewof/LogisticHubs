using Application.DTOs.Common;
using Application.DTOs.Products;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Products
{
    public class ProductVariantUseCase
    {
        private readonly IProductVariantRepository _variantRepository;
        private readonly IProductRepository _productRepository;

        public ProductVariantUseCase(
            IProductVariantRepository variantRepository,
            IProductRepository productRepository)
        {
            _variantRepository = variantRepository;
            _productRepository = productRepository;
        }

        public async Task<PagedResult<ProductVariantDto>> GetPagedAsync(PagedRequest request, Guid? productId = null)
        {
            var result = await _variantRepository.GetPagedAsync(request, productId);

            return new PagedResult<ProductVariantDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<IEnumerable<ProductVariantDto>> GetByProductIdAsync(Guid productId)
        {
            var variants = await _variantRepository.GetByProductIdAsync(productId);
            return variants.Select(MapToDto);
        }

        public async Task<ProductVariantDto?> GetByIdAsync(Guid id)
        {
            var variant = await _variantRepository.GetByIdAsync(id);
            return variant == null ? null : MapToDto(variant);
        }

        public async Task<ProductVariantDto> CreateAsync(CreateProductVariantRequest request)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId)
                ?? throw new InvalidOperationException("Product not found.");

            if (await _variantRepository.SkuExistsAsync(request.Sku))
                throw new InvalidOperationException($"A variant with SKU '{request.Sku}' already exists.");

            if (!string.IsNullOrWhiteSpace(request.Barcode) && await _variantRepository.BarcodeExistsAsync(request.Barcode))
                throw new InvalidOperationException($"A variant with barcode '{request.Barcode}' already exists.");

            var variant = new ProductVariant
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                Sku = request.Sku,
                Name = request.Name,
                VariantAttributes = request.VariantAttributes,
                Barcode = request.Barcode,
                CostPrice = request.CostPrice,
                SellingPrice = request.SellingPrice,
                Weight = request.Weight,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _variantRepository.CreateAsync(variant);
            return MapToDto(created);
        }

        public async Task UpdateAsync(Guid id, UpdateProductVariantRequest request)
        {
            var variant = await _variantRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Product variant not found.");

            if (request.Sku != null) variant.Sku = request.Sku;
            if (request.Name != null) variant.Name = request.Name;
            if (request.VariantAttributes != null) variant.VariantAttributes = request.VariantAttributes;
            if (request.Barcode != null) variant.Barcode = request.Barcode;
            if (request.CostPrice.HasValue) variant.CostPrice = request.CostPrice.Value;
            if (request.SellingPrice.HasValue) variant.SellingPrice = request.SellingPrice.Value;
            if (request.Weight.HasValue) variant.Weight = request.Weight.Value;
            if (request.IsActive.HasValue) variant.IsActive = request.IsActive.Value;

            variant.UpdatedAt = DateTime.UtcNow;

            await _variantRepository.UpdateAsync(variant);
        }

        public async Task DeleteAsync(Guid id)
        {
            var variant = await _variantRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Product variant not found.");

            await _variantRepository.DeleteAsync(variant);
        }

        private static ProductVariantDto MapToDto(ProductVariant v) => new()
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
    }
}
