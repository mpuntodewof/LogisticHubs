using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IProductReviewRepository
    {
        Task<PagedResult<ProductReview>> GetPagedByProductIdAsync(Guid productId, PagedRequest request, string? status = null);
        Task<ProductReview?> GetByIdAsync(Guid id);
        Task<ProductReview?> GetByProductAndCustomerAsync(Guid productId, Guid customerId);
        Task<ProductReview> CreateAsync(ProductReview review);
        Task UpdateAsync(ProductReview review);
        Task DeleteAsync(ProductReview review);
        Task<(decimal AverageRating, int ReviewCount)> GetAverageRatingAsync(Guid productId);
    }
}
