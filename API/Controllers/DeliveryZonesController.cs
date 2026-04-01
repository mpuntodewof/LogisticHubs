using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Logistics;
using Application.UseCases.Logistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/delivery-zones")]
    [Authorize]
    public class DeliveryZonesController : ControllerBase
    {
        private readonly DeliveryZoneUseCase _deliveryZoneUseCase;

        public DeliveryZonesController(DeliveryZoneUseCase deliveryZoneUseCase)
        {
            _deliveryZoneUseCase = deliveryZoneUseCase;
        }

        /// <summary>Get all delivery zones (paginated).</summary>
        [HttpGet]
        [RequirePermission("delivery-zones.read")]
        public async Task<ActionResult<PagedResult<DeliveryZoneDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _deliveryZoneUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a delivery zone by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("delivery-zones.read")]
        public async Task<ActionResult<DeliveryZoneDto>> GetById(Guid id)
        {
            var zone = await _deliveryZoneUseCase.GetByIdAsync(id);
            if (zone == null) return NotFound();
            return Ok(zone);
        }

        /// <summary>Create a new delivery zone.</summary>
        [HttpPost]
        [RequirePermission("delivery-zones.create")]
        public async Task<ActionResult<DeliveryZoneDto>> Create([FromBody] CreateDeliveryZoneRequest request)
        {
            try
            {
                var zone = await _deliveryZoneUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = zone.Id }, zone);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a delivery zone.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("delivery-zones.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDeliveryZoneRequest request)
        {
            try
            {
                await _deliveryZoneUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Delete a delivery zone.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("delivery-zones.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _deliveryZoneUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
