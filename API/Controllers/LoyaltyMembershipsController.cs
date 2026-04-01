using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Loyalty;
using Application.UseCases.Loyalty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/loyalty-memberships")]
    [Authorize]
    public class LoyaltyMembershipsController : ControllerBase
    {
        private readonly LoyaltyMembershipUseCase _membershipUseCase;

        public LoyaltyMembershipsController(LoyaltyMembershipUseCase membershipUseCase)
        {
            _membershipUseCase = membershipUseCase;
        }

        /// <summary>Get all loyalty memberships (paginated, filterable).</summary>
        [HttpGet]
        [RequirePermission("loyalty.read")]
        public async Task<ActionResult<PagedResult<LoyaltyMembershipDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] Guid? programId,
            [FromQuery] Guid? customerId)
        {
            var result = await _membershipUseCase.GetPagedAsync(request, programId, customerId);
            return Ok(result);
        }

        /// <summary>Get a loyalty membership by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("loyalty.read")]
        public async Task<ActionResult<LoyaltyMembershipDto>> GetById(Guid id)
        {
            var membership = await _membershipUseCase.GetByIdAsync(id);
            if (membership == null) return NotFound();
            return Ok(membership);
        }

        /// <summary>Enroll a customer in a loyalty program.</summary>
        [HttpPost("enroll")]
        [RequirePermission("loyalty.enroll")]
        public async Task<ActionResult<LoyaltyMembershipDto>> Enroll([FromBody] EnrollCustomerRequest request)
        {
            try
            {
                var membership = await _membershipUseCase.EnrollAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = membership.Id }, membership);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Get transactions for a membership.</summary>
        [HttpGet("{id:guid}/transactions")]
        [RequirePermission("loyalty.read")]
        public async Task<ActionResult<PagedResult<LoyaltyPointTransactionDto>>> GetTransactions(
            Guid id, [FromQuery] PagedRequest request)
        {
            var result = await _membershipUseCase.GetTransactionsAsync(id, request);
            return Ok(result);
        }

        /// <summary>Adjust points manually (admin).</summary>
        [HttpPost("{id:guid}/adjust")]
        [RequirePermission("loyalty.adjust")]
        public async Task<ActionResult<LoyaltyPointTransactionDto>> AdjustPoints(
            Guid id, [FromBody] AdjustPointsRequest request)
        {
            try
            {
                var transaction = await _membershipUseCase.AdjustPointsAsync(id, request);
                return Ok(transaction);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Redeem points for a sales order.</summary>
        [HttpPost("{id:guid}/redeem")]
        [RequirePermission("loyalty.redeem")]
        public async Task<ActionResult<LoyaltyPointTransactionDto>> RedeemPoints(
            Guid id, [FromBody] RedeemPointsRequest request)
        {
            try
            {
                var transaction = await _membershipUseCase.RedeemPointsAsync(id, request);
                return Ok(transaction);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
