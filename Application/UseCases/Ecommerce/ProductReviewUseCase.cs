using Application.DTOs.Common;
using Application.DTOs.Ecommerce;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Ecommerce
{
    public class ProductReviewUseCase
    {
        private readonly IProductReviewRepository _reviewRepository;

        public ProductReviewUseCase(IProductReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<PagedResult<ProductReviewDto>> GetPagedByProductIdAsync(Guid productId, PagedRequest request, string? status = null)
        {
            var result = await _reviewRepository.GetPagedByProductIdAsync(productId, request, status);

            return new PagedResult<ProductReviewDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<ProductReviewDto?> GetByIdAsync(Guid id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            return review == null ? null : MapToDto(review);
        }

        public async Task<ProductReviewDto> CreateAsync(Guid customerId, CreateProductReviewRequest request)
        {
            // Validate no duplicate review
            var existing = await _reviewRepository.GetByProductAndCustomerAsync(request.ProductId, customerId);
            if (existing != null)
                throw new InvalidOperationException("You have already reviewed this product.");

            var isVerifiedPurchase = request.SalesOrderId.HasValue;

            var review = new ProductReview
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                CustomerId = customerId,
                SalesOrderId = request.SalesOrderId,
                Rating = request.Rating,
                Title = request.Title,
                Comment = request.Comment,
                Status = ReviewStatus.Pending.ToString(),
                IsVerifiedPurchase = isVerifiedPurchase,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _reviewRepository.CreateAsync(review);

            // Re-fetch to get navigation properties
            var fetched = await _reviewRepository.GetByIdAsync(created.Id);
            return MapToDto(fetched!);
        }

        public async Task<ProductReviewDto> ModerateAsync(Guid id, ModerateReviewRequest request)
        {
            var review = await _reviewRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Product review not found.");

            review.Status = request.Status.ToString();
            review.AdminResponse = request.AdminResponse;
            review.AdminRespondedAt = DateTime.UtcNow;
            review.UpdatedAt = DateTime.UtcNow;

            await _reviewRepository.UpdateAsync(review);

            var updated = await _reviewRepository.GetByIdAsync(id);
            return MapToDto(updated!);
        }

        public async Task DeleteAsync(Guid id)
        {
            var review = await _reviewRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Product review not found.");

            await _reviewRepository.DeleteAsync(review);
        }

        public async Task<ProductRatingSummaryDto> GetRatingSummaryAsync(Guid productId)
        {
            var (averageRating, reviewCount) = await _reviewRepository.GetAverageRatingAsync(productId);

            return new ProductRatingSummaryDto
            {
                ProductId = productId,
                AverageRating = averageRating,
                ReviewCount = reviewCount
            };
        }

        private static ProductReviewDto MapToDto(ProductReview r) => new()
        {
            Id = r.Id,
            ProductId = r.ProductId,
            ProductName = r.Product?.Name ?? string.Empty,
            CustomerId = r.CustomerId,
            CustomerName = r.Customer?.Name ?? string.Empty,
            Rating = r.Rating,
            Title = r.Title,
            Comment = r.Comment,
            Status = r.Status,
            IsVerifiedPurchase = r.IsVerifiedPurchase,
            AdminResponse = r.AdminResponse,
            AdminRespondedAt = r.AdminRespondedAt,
            CreatedAt = r.CreatedAt
        };
    }
}
