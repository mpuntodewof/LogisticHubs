using Application.DTOs.Products;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Products
{
    public class ProductImageUseCase
    {
        private readonly IProductImageRepository _imageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductImageUseCase(IProductImageRepository imageRepository, IUnitOfWork unitOfWork)
        {
            _imageRepository = imageRepository;
            _unitOfWork = unitOfWork;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<ProductImageDto>> GetByProductIdAsync(Guid productId)
        {
            var images = await _imageRepository.GetByProductIdAsync(productId);
            return images.Select(MapToDto);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<ProductImageDto> CreateAsync(CreateProductImageRequest request)
        {
            var image = new ProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                ProductVariantId = request.ProductVariantId,
                ImageUrl = request.ImageUrl,
                AltText = request.AltText,
                SortOrder = request.SortOrder,
                IsPrimary = request.IsPrimary,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _imageRepository.CreateAsync(image);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(created);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var image = await _imageRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Product image {id} not found.");

            await _imageRepository.DeleteAsync(image);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static ProductImageDto MapToDto(ProductImage i) => new()
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductVariantId = i.ProductVariantId,
            ImageUrl = i.ImageUrl,
            AltText = i.AltText,
            SortOrder = i.SortOrder,
            IsPrimary = i.IsPrimary
        };
    }
}
