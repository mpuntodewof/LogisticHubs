using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Promotions;
using Application.UseCases.Promotions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/promotions")]
    [Authorize]
    public class PromotionsController : ControllerBase
    {
        private readonly PromotionUseCase _promotionUseCase;

        public PromotionsController(PromotionUseCase promotionUseCase)
        {
            _promotionUseCase = promotionUseCase;
        }

        /// <summary>Get all promotions (paginated).</summary>
        [HttpGet]
        [RequirePermission("promotions.read")]
        public async Task<ActionResult<PagedResult<PromotionDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] string? status,
            [FromQuery] string? type)
        {
            var result = await _promotionUseCase.GetPagedAsync(request, status, type);
            return Ok(result);
        }

        /// <summary>Get a promotion by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("promotions.read")]
        public async Task<ActionResult<PromotionDetailDto>> GetById(Guid id)
        {
            var promotion = await _promotionUseCase.GetByIdAsync(id);
            if (promotion == null) return NotFound();
            return Ok(promotion);
        }

        /// <summary>Create a new promotion.</summary>
        [HttpPost]
        [RequirePermission("promotions.create")]
        public async Task<ActionResult<PromotionDto>> Create([FromBody] CreatePromotionRequest request)
        {
            try
            {
                var promotion = await _promotionUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = promotion.Id }, promotion);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a promotion.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("promotions.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePromotionRequest request)
        {
            try
            {
                await _promotionUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Delete a promotion.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("promotions.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _promotionUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Activate a promotion.</summary>
        [HttpPost("{id:guid}/activate")]
        [RequirePermission("promotions.activate")]
        public async Task<IActionResult> Activate(Guid id)
        {
            try
            {
                await _promotionUseCase.ActivateAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Pause a promotion.</summary>
        [HttpPost("{id:guid}/pause")]
        [RequirePermission("promotions.activate")]
        public async Task<IActionResult> Pause(Guid id)
        {
            try
            {
                await _promotionUseCase.PauseAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Cancel a promotion.</summary>
        [HttpPost("{id:guid}/cancel")]
        [RequirePermission("promotions.activate")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            try
            {
                await _promotionUseCase.CancelAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Add a rule to a promotion.</summary>
        [HttpPost("{id:guid}/rules")]
        [RequirePermission("promotions.update")]
        public async Task<ActionResult<PromotionRuleDto>> AddRule(Guid id, [FromBody] CreatePromotionRuleRequest request)
        {
            try
            {
                var rule = await _promotionUseCase.AddRuleAsync(id, request);
                return Ok(rule);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Remove a rule from a promotion.</summary>
        [HttpDelete("{id:guid}/rules/{ruleId:guid}")]
        [RequirePermission("promotions.update")]
        public async Task<IActionResult> RemoveRule(Guid id, Guid ruleId)
        {
            try
            {
                await _promotionUseCase.RemoveRuleAsync(id, ruleId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Add a product to a promotion.</summary>
        [HttpPost("{id:guid}/products")]
        [RequirePermission("promotions.update")]
        public async Task<ActionResult<PromotionProductDto>> AddProduct(Guid id, [FromBody] CreatePromotionProductRequest request)
        {
            try
            {
                var product = await _promotionUseCase.AddProductAsync(id, request);
                return Ok(product);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Remove a product from a promotion.</summary>
        [HttpDelete("{id:guid}/products/{ppId:guid}")]
        [RequirePermission("promotions.update")]
        public async Task<IActionResult> RemoveProduct(Guid id, Guid ppId)
        {
            try
            {
                await _promotionUseCase.RemoveProductAsync(id, ppId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
