using Asp.Versioning;
using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Finance;
using Application.UseCases.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/payment-terms")]
    [Authorize]
    public class PaymentTermsController : ControllerBase
    {
        private readonly PaymentTermUseCase _useCase;

        public PaymentTermsController(PaymentTermUseCase useCase)
        {
            _useCase = useCase;
        }

        /// <summary>Get all payment terms (paginated).</summary>
        [HttpGet]
        [RequirePermission("payment-terms.read")]
        public async Task<ActionResult<PagedResult<PaymentTermDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _useCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a payment term by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("payment-terms.read")]
        public async Task<ActionResult<PaymentTermDto>> GetById(Guid id)
        {
            var term = await _useCase.GetByIdAsync(id);
            if (term == null) return NotFound();
            return Ok(term);
        }

        /// <summary>Create a new payment term.</summary>
        [HttpPost]
        [RequirePermission("payment-terms.create")]
        public async Task<ActionResult<PaymentTermDto>> Create([FromBody] CreatePaymentTermRequest request)
        {
            try
            {
                var term = await _useCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = term.Id }, term);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a payment term.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("payment-terms.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePaymentTermRequest request)
        {
            try
            {
                await _useCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Delete a payment term.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("payment-terms.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _useCase.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
