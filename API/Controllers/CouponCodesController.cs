using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Ecommerce;
using Application.UseCases.Ecommerce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/coupons")]
    [Authorize]
    public class CouponCodesController : ControllerBase
    {
        private readonly CouponCodeUseCase _couponUseCase;

        public CouponCodesController(CouponCodeUseCase couponUseCase)
        {
            _couponUseCase = couponUseCase;
        }

        /// <summary>Get all coupon codes (paginated).</summary>
        [HttpGet]
        [RequirePermission("coupons.read")]
        public async Task<ActionResult<PagedResult<CouponCodeDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] bool? isActive)
        {
            var result = await _couponUseCase.GetPagedAsync(request, isActive);
            return Ok(result);
        }

        /// <summary>Get a coupon code by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("coupons.read")]
        public async Task<ActionResult<CouponCodeDto>> GetById(Guid id)
        {
            var coupon = await _couponUseCase.GetByIdAsync(id);
            if (coupon == null) return NotFound();
            return Ok(coupon);
        }

        /// <summary>Create a new coupon code.</summary>
        [HttpPost]
        [RequirePermission("coupons.create")]
        public async Task<ActionResult<CouponCodeDto>> Create([FromBody] CreateCouponCodeRequest request)
        {
            try
            {
                var coupon = await _couponUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = coupon.Id }, coupon);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a coupon code.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("coupons.update")]
        public async Task<ActionResult<CouponCodeDto>> Update(Guid id, [FromBody] UpdateCouponCodeRequest request)
        {
            try
            {
                var coupon = await _couponUseCase.UpdateAsync(id, request);
                return Ok(coupon);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Delete a coupon code.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("coupons.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _couponUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Validate a coupon code.</summary>
        [HttpPost("validate")]
        [RequirePermission("coupons.read")]
        public async Task<ActionResult<ValidateCouponResponse>> Validate(
            [FromBody] ValidateCouponRequest request,
            [FromQuery] Guid? customerId)
        {
            var result = await _couponUseCase.ValidateAsync(request, customerId);
            return Ok(result);
        }

        /// <summary>Get usages for a coupon code.</summary>
        [HttpGet("{id:guid}/usages")]
        [RequirePermission("coupons.read")]
        public async Task<ActionResult<PagedResult<CouponUsageDto>>> GetUsages(Guid id, [FromQuery] PagedRequest request)
        {
            var result = await _couponUseCase.GetUsagesAsync(id, request);
            return Ok(result);
        }
    }
}
