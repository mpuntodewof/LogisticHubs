using API.Filters;
using Application.DTOs.Shipments;
using Application.UseCases.Shipments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/shipments")]
    [Authorize]
    public class ShipmentsController : ControllerBase
    {
        private readonly ShipmentUseCase _shipmentUseCase;

        public ShipmentsController(ShipmentUseCase shipmentUseCase)
        {
            _shipmentUseCase = shipmentUseCase;
        }

        /// <summary>Get all shipments.</summary>
        [HttpGet]
        [RequirePermission("shipments.read")]
        public async Task<ActionResult<IEnumerable<ShipmentDto>>> GetAll()
        {
            var shipments = await _shipmentUseCase.GetAllAsync();
            return Ok(shipments);
        }

        /// <summary>Get a shipment by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("shipments.read")]
        public async Task<ActionResult<ShipmentDto>> GetById(Guid id)
        {
            var shipment = await _shipmentUseCase.GetByIdAsync(id);
            if (shipment == null) return NotFound();
            return Ok(shipment);
        }

        /// <summary>Get a shipment by tracking number.</summary>
        [HttpGet("track/{trackingNumber}")]
        [RequirePermission("shipments.read")]
        public async Task<ActionResult<ShipmentDto>> GetByTrackingNumber(string trackingNumber)
        {
            var shipment = await _shipmentUseCase.GetByTrackingNumberAsync(trackingNumber);
            if (shipment == null) return NotFound();
            return Ok(shipment);
        }

        /// <summary>Create a new shipment.</summary>
        [HttpPost]
        [RequirePermission("shipments.create")]
        public async Task<ActionResult<ShipmentDto>> Create([FromBody] CreateShipmentRequest request)
        {
            var shipment = await _shipmentUseCase.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, shipment);
        }

        /// <summary>Update a shipment's details or status.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("shipments.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateShipmentRequest request)
        {
            try
            {
                await _shipmentUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Delete a shipment (only Pending or Cancelled).</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("shipments.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _shipmentUseCase.DeleteAsync(id);
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

        /// <summary>Assign a driver and vehicle to a shipment.</summary>
        [HttpPost("{id:guid}/assign")]
        [RequirePermission("shipments.assign")]
        public async Task<IActionResult> Assign(Guid id, [FromBody] AssignShipmentRequest request)
        {
            try
            {
                await _shipmentUseCase.AssignAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Get all tracking events for a shipment.</summary>
        [HttpGet("{id:guid}/tracking")]
        [RequirePermission("tracking.read")]
        public async Task<ActionResult<IEnumerable<TrackingEventDto>>> GetTracking(Guid id)
        {
            try
            {
                var events = await _shipmentUseCase.GetTrackingAsync(id);
                return Ok(events);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Add a tracking event to a shipment.</summary>
        [HttpPost("{id:guid}/tracking")]
        [RequirePermission("tracking.create")]
        public async Task<ActionResult<TrackingEventDto>> AddTracking(Guid id, [FromBody] AddTrackingEventRequest request)
        {
            try
            {
                var trackingEvent = await _shipmentUseCase.AddTrackingEventAsync(id, request);
                return Ok(trackingEvent);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
