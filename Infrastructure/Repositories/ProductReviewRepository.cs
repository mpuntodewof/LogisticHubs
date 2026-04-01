using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly AppDbContext _context;

        public ProductReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ProductReview>> GetPagedByProductIdAsync(Guid productId, PagedRequest request, string? status = null)
        {
            var query = _context.ProductReviews
                .Include(r => r.Product)
                .Include(r => r.Customer)
                .Where(r => r.ProductId == productId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(r => r.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(r =>
                    (r.Title != null && r.Title.ToLower().Contains(search)) ||
                    (r.Comment != null && r.Comment.ToLower().Contains(search)));
            }

            query = request.SortBy?.ToLower() switch
            {
                "rating" => request.SortDescending ? query.OrderByDescending(r => r.Rating) : query.OrderBy(r => r.Rating),
                "status" => request.SortDescending ? query.OrderByDescending(r => r.Status) : query.OrderBy(r => r.Status),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<ProductReview?> GetByIdAsync(Guid id)
            => await _context.ProductReviews
                .Include(r => r.Product)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<ProductReview?> GetByProductAndCustomerAsync(Guid productId, Guid customerId)
            => await _context.ProductReviews
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.CustomerId == customerId);

        public async Task<ProductReview> CreateAsync(ProductReview review)
        {
            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task UpdateAsync(ProductReview review)
        {
            _context.ProductReviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductReview review)
        {
            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync();
        }

        public async Task<(decimal AverageRating, int ReviewCount)> GetAverageRatingAsync(Guid productId)
        {
            var reviews = _context.ProductReviews
                .Where(r => r.ProductId == productId && r.Status == "Approved");

            var count = await reviews.CountAsync();
            if (count == 0)
                return (0m, 0);

            var average = await reviews.AverageAsync(r => (decimal)r.Rating);
            return (Math.Round(average, 2), count);
        }
    }
}
