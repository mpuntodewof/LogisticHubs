using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Storefront;
using Application.UseCases.Storefront;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/admin/banners")]
    [Authorize]
    public class BannersController : ControllerBase
    {
        private readonly BannerUseCase _bannerUseCase;

        public BannersController(BannerUseCase bannerUseCase)
        {
            _bannerUseCase = bannerUseCase;
        }

        /// <summary>Get all banners (paginated).</summary>
        [HttpGet]
        [RequirePermission("banners.read")]
        public async Task<ActionResult<PagedResult<BannerDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] string? position = null,
            [FromQuery] bool? isActive = null)
        {
            var result = await _bannerUseCase.GetPagedAsync(request, position, isActive);
            return Ok(result);
        }

        /// <summary>Get a banner by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("banners.read")]
        public async Task<ActionResult<BannerDto>> GetById(Guid id)
        {
            var banner = await _bannerUseCase.GetByIdAsync(id);
            if (banner == null) return NotFound();
            return Ok(banner);
        }

        /// <summary>Create a new banner.</summary>
        [HttpPost]
        [RequirePermission("banners.create")]
        public async Task<ActionResult<BannerDto>> Create([FromBody] CreateBannerRequest request)
        {
            var banner = await _bannerUseCase.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = banner.Id }, banner);
        }

        /// <summary>Update a banner.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("banners.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBannerRequest request)
        {
            try
            {
                await _bannerUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Delete a banner.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("banners.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _bannerUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
