using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.PaymentGateway;
using Application.UseCases.PaymentGateway;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/payment-gateway-configs")]
    [Authorize]
    public class PaymentGatewayConfigsController : ControllerBase
    {
        private readonly PaymentGatewayConfigUseCase _useCase;

        public PaymentGatewayConfigsController(PaymentGatewayConfigUseCase useCase)
        {
            _useCase = useCase;
        }

        /// <summary>Get all payment gateway configs (paginated).</summary>
        [HttpGet]
        [RequirePermission("payment-gateways.read")]
        public async Task<ActionResult<PagedResult<PaymentGatewayConfigDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _useCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a payment gateway config by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("payment-gateways.read")]
        public async Task<ActionResult<PaymentGatewayConfigDto>> GetById(Guid id)
        {
            var config = await _useCase.GetByIdAsync(id);
            if (config == null) return NotFound();
            return Ok(config);
        }

        /// <summary>Create a new payment gateway config.</summary>
        [HttpPost]
        [RequirePermission("payment-gateways.create")]
        public async Task<ActionResult<PaymentGatewayConfigDto>> Create([FromBody] CreatePaymentGatewayConfigRequest request)
        {
            try
            {
                var config = await _useCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = config.Id }, config);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a payment gateway config.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("payment-gateways.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePaymentGatewayConfigRequest request)
        {
            try
            {
                await _useCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Delete a payment gateway config.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("payment-gateways.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _useCase.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
