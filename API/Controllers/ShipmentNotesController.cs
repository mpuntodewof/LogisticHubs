using API.Filters;
using Application.DTOs.Logistics;
using Application.UseCases.Logistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/shipments/{shipmentId:guid}/notes")]
    [Authorize]
    public class ShipmentNotesController : ControllerBase
    {
        private readonly ShipmentNoteUseCase _shipmentNoteUseCase;

        public ShipmentNotesController(ShipmentNoteUseCase shipmentNoteUseCase)
        {
            _shipmentNoteUseCase = shipmentNoteUseCase;
        }

        /// <summary>Get all notes for a shipment.</summary>
        [HttpGet]
        [RequirePermission("shipment-notes.read")]
        public async Task<ActionResult<IEnumerable<ShipmentNoteDto>>> GetByShipmentId(Guid shipmentId)
        {
            var notes = await _shipmentNoteUseCase.GetByShipmentIdAsync(shipmentId);
            return Ok(notes);
        }

        /// <summary>Create a note for a shipment.</summary>
        [HttpPost]
        [RequirePermission("shipment-notes.create")]
        public async Task<ActionResult<ShipmentNoteDto>> Create(Guid shipmentId, [FromBody] CreateShipmentNoteRequest request)
        {
            var note = await _shipmentNoteUseCase.CreateAsync(shipmentId, request);
            return Created($"api/shipments/{shipmentId}/notes/{note.Id}", note);
        }

        /// <summary>Delete a shipment note.</summary>
        [HttpDelete("{noteId:guid}")]
        [RequirePermission("shipment-notes.delete")]
        public async Task<IActionResult> Delete(Guid shipmentId, Guid noteId)
        {
            try
            {
                await _shipmentNoteUseCase.DeleteAsync(noteId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
