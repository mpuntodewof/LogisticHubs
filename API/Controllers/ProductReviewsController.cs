using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Ecommerce;
using Application.UseCases.Ecommerce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/product-reviews")]
    [Authorize]
    public class ProductReviewsController : ControllerBase
    {
        private readonly ProductReviewUseCase _reviewUseCase;

        public ProductReviewsController(ProductReviewUseCase reviewUseCase)
        {
            _reviewUseCase = reviewUseCase;
        }

        /// <summary>Get product reviews (paginated, filtered by productId and optional status).</summary>
        [HttpGet]
        [RequirePermission("product-reviews.read")]
        public async Task<ActionResult<PagedResult<ProductReviewDto>>> GetAll(
            [FromQuery] Guid productId,
            [FromQuery] PagedRequest request,
            [FromQuery] string? status)
        {
            var result = await _reviewUseCase.GetPagedByProductIdAsync(productId, request, status);
            return Ok(result);
        }

        /// <summary>Get a product review by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("product-reviews.read")]
        public async Task<ActionResult<ProductReviewDto>> GetById(Guid id)
        {
            var review = await _reviewUseCase.GetByIdAsync(id);
            if (review == null) return NotFound();
            return Ok(review);
        }

        /// <summary>Get rating summary for a product.</summary>
        [HttpGet("summary/{productId:guid}")]
        [RequirePermission("product-reviews.read")]
        public async Task<ActionResult<ProductRatingSummaryDto>> GetRatingSummary(Guid productId)
        {
            var summary = await _reviewUseCase.GetRatingSummaryAsync(productId);
            return Ok(summary);
        }

        /// <summary>Moderate a product review (approve/reject).</summary>
        [HttpPost("{id:guid}/moderate")]
        [RequirePermission("product-reviews.moderate")]
        public async Task<ActionResult<ProductReviewDto>> Moderate(Guid id, [FromBody] ModerateReviewRequest request)
        {
            try
            {
                var review = await _reviewUseCase.ModerateAsync(id, request);
                return Ok(review);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Delete a product review.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("product-reviews.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _reviewUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
