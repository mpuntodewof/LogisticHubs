using Application.DTOs.Common;
using Application.DTOs.Ecommerce;
using Application.DTOs.Products;
using Application.DTOs.Storefront;
using Application.UseCases.Ecommerce;
using Application.UseCases.Products;
using Application.UseCases.Storefront;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/storefront")]
    public class StorefrontController : ControllerBase
    {
        private readonly StorefrontConfigUseCase _storefrontConfigUseCase;
        private readonly BannerUseCase _bannerUseCase;
        private readonly PageUseCase _pageUseCase;
        private readonly ProductUseCase _productUseCase;
        private readonly ProductReviewUseCase _productReviewUseCase;

        public StorefrontController(
            StorefrontConfigUseCase storefrontConfigUseCase,
            BannerUseCase bannerUseCase,
            PageUseCase pageUseCase,
            ProductUseCase productUseCase,
            ProductReviewUseCase productReviewUseCase)
        {
            _storefrontConfigUseCase = storefrontConfigUseCase;
            _bannerUseCase = bannerUseCase;
            _pageUseCase = pageUseCase;
            _productUseCase = productUseCase;
            _productReviewUseCase = productReviewUseCase;
        }

        /// <summary>Get public storefront configuration (branding/SEO).</summary>
        [HttpGet("config")]
        public async Task<ActionResult<StorefrontConfigDto>> GetConfig()
        {
            var result = await _storefrontConfigUseCase.GetAsync();
            return Ok(result);
        }

        /// <summary>Get active banners, optionally filtered by position.</summary>
        [HttpGet("banners")]
        public async Task<ActionResult<IEnumerable<BannerDto>>> GetBanners([FromQuery] string? position = null)
        {
            var result = await _bannerUseCase.GetActiveBannersAsync(position);
            return Ok(result);
        }

        /// <summary>Get a published page by slug.</summary>
        [HttpGet("pages/{slug}")]
        public async Task<ActionResult<PageDetailDto>> GetPage(string slug)
        {
            var page = await _pageUseCase.GetBySlugAsync(slug);
            if (page == null || page.Status != "Published")
                return NotFound();
            return Ok(page);
        }

        /// <summary>Browse products (public).</summary>
        [HttpGet("products")]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts([FromQuery] PagedRequest request)
        {
            var result = await _productUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get product detail (public).</summary>
        [HttpGet("products/{id:guid}")]
        public async Task<ActionResult<ProductDetailDto>> GetProduct(Guid id)
        {
            var product = await _productUseCase.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        /// <summary>Get approved reviews for a product.</summary>
        [HttpGet("products/{id:guid}/reviews")]
        public async Task<ActionResult<PagedResult<ProductReviewDto>>> GetProductReviews(
            Guid id,
            [FromQuery] PagedRequest request)
        {
            var result = await _productReviewUseCase.GetPagedByProductIdAsync(id, request, "Approved");
            return Ok(result);
        }

        /// <summary>Get rating summary for a product.</summary>
        [HttpGet("products/{id:guid}/rating")]
        public async Task<ActionResult<ProductRatingSummaryDto>> GetProductRating(Guid id)
        {
            var result = await _productReviewUseCase.GetRatingSummaryAsync(id);
            return Ok(result);
        }
    }
}
