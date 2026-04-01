using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Loyalty;
using Application.UseCases.Loyalty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/loyalty-programs")]
    [Authorize]
    public class LoyaltyProgramsController : ControllerBase
    {
        private readonly LoyaltyProgramUseCase _loyaltyProgramUseCase;

        public LoyaltyProgramsController(LoyaltyProgramUseCase loyaltyProgramUseCase)
        {
            _loyaltyProgramUseCase = loyaltyProgramUseCase;
        }

        /// <summary>Get all loyalty programs (paginated).</summary>
        [HttpGet]
        [RequirePermission("loyalty.read")]
        public async Task<ActionResult<PagedResult<LoyaltyProgramDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _loyaltyProgramUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a loyalty program by ID (includes tiers).</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("loyalty.read")]
        public async Task<ActionResult<LoyaltyProgramDetailDto>> GetById(Guid id)
        {
            var program = await _loyaltyProgramUseCase.GetByIdAsync(id);
            if (program == null) return NotFound();
            return Ok(program);
        }

        /// <summary>Create a new loyalty program.</summary>
        [HttpPost]
        [RequirePermission("loyalty.create")]
        public async Task<ActionResult<LoyaltyProgramDto>> Create([FromBody] CreateLoyaltyProgramRequest request)
        {
            try
            {
                var program = await _loyaltyProgramUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = program.Id }, program);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a loyalty program.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("loyalty.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLoyaltyProgramRequest request)
        {
            try
            {
                await _loyaltyProgramUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Delete a loyalty program.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("loyalty.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _loyaltyProgramUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Get tiers for a loyalty program.</summary>
        [HttpGet("{id:guid}/tiers")]
        [RequirePermission("loyalty.read")]
        public async Task<ActionResult<IEnumerable<LoyaltyTierDto>>> GetTiers(Guid id)
        {
            var tiers = await _loyaltyProgramUseCase.GetTiersByProgramAsync(id);
            return Ok(tiers);
        }

        /// <summary>Create a tier for a loyalty program.</summary>
        [HttpPost("{id:guid}/tiers")]
        [RequirePermission("loyalty.create")]
        public async Task<ActionResult<LoyaltyTierDto>> CreateTier(Guid id, [FromBody] CreateLoyaltyTierRequest request)
        {
            try
            {
                var tier = await _loyaltyProgramUseCase.CreateTierAsync(id, request);
                return Ok(tier);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a tier in a loyalty program.</summary>
        [HttpPut("{id:guid}/tiers/{tierId:guid}")]
        [RequirePermission("loyalty.update")]
        public async Task<IActionResult> UpdateTier(Guid id, Guid tierId, [FromBody] UpdateLoyaltyTierRequest request)
        {
            try
            {
                await _loyaltyProgramUseCase.UpdateTierAsync(id, tierId, request);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Delete a tier from a loyalty program.</summary>
        [HttpDelete("{id:guid}/tiers/{tierId:guid}")]
        [RequirePermission("loyalty.delete")]
        public async Task<IActionResult> DeleteTier(Guid id, Guid tierId)
        {
            try
            {
                await _loyaltyProgramUseCase.DeleteTierAsync(id, tierId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
