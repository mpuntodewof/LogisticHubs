using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Logistics;
using Application.UseCases.Logistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/delivery-rates")]
    [Authorize]
    public class DeliveryRatesController : ControllerBase
    {
        private readonly DeliveryRateUseCase _deliveryRateUseCase;

        public DeliveryRatesController(DeliveryRateUseCase deliveryRateUseCase)
        {
            _deliveryRateUseCase = deliveryRateUseCase;
        }

        /// <summary>Get all delivery rates (paginated, optionally filtered by zone).</summary>
        [HttpGet]
        [RequirePermission("delivery-rates.read")]
        public async Task<ActionResult<PagedResult<DeliveryRateDto>>> GetAll([FromQuery] PagedRequest request, [FromQuery] Guid? zoneId = null)
        {
            var result = await _deliveryRateUseCase.GetPagedAsync(request, zoneId);
            return Ok(result);
        }

        /// <summary>Get a delivery rate by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("delivery-rates.read")]
        public async Task<ActionResult<DeliveryRateDto>> GetById(Guid id)
        {
            var rate = await _deliveryRateUseCase.GetByIdAsync(id);
            if (rate == null) return NotFound();
            return Ok(rate);
        }

        /// <summary>Create a new delivery rate.</summary>
        [HttpPost]
        [RequirePermission("delivery-rates.create")]
        public async Task<ActionResult<DeliveryRateDto>> Create([FromBody] CreateDeliveryRateRequest request)
        {
            var rate = await _deliveryRateUseCase.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = rate.Id }, rate);
        }

        /// <summary>Update a delivery rate.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("delivery-rates.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDeliveryRateRequest request)
        {
            try
            {
                await _deliveryRateUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Delete a delivery rate.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("delivery-rates.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _deliveryRateUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Calculate shipping cost for a delivery zone and weight.</summary>
        [HttpPost("calculate")]
        [RequirePermission("delivery-rates.read")]
        public async Task<ActionResult<CalculateShippingResponse>> Calculate([FromBody] CalculateShippingRequest request)
        {
            try
            {
                var result = await _deliveryRateUseCase.CalculateShippingAsync(request);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
