using Application.DTOs.Logistics;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Logistics
{
    public class ShipmentNoteUseCase
    {
        private readonly IShipmentNoteRepository _shipmentNoteRepository;

        public ShipmentNoteUseCase(IShipmentNoteRepository shipmentNoteRepository)
        {
            _shipmentNoteRepository = shipmentNoteRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<ShipmentNoteDto>> GetByShipmentIdAsync(Guid shipmentId)
        {
            var notes = await _shipmentNoteRepository.GetByShipmentIdAsync(shipmentId);
            return notes.Select(MapToDto);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<ShipmentNoteDto> CreateAsync(Guid shipmentId, CreateShipmentNoteRequest request)
        {
            var note = new ShipmentNote
            {
                Id = Guid.NewGuid(),
                ShipmentId = shipmentId,
                NoteType = request.NoteType.ToString(),
                Content = request.Content,
                AttachmentUrl = request.AttachmentUrl,
                AttachmentFileName = request.AttachmentFileName,
                IsInternal = request.IsInternal,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _shipmentNoteRepository.CreateAsync(note);
            return MapToDto(created);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid noteId)
        {
            var note = await _shipmentNoteRepository.GetByIdAsync(noteId)
                ?? throw new KeyNotFoundException($"ShipmentNote {noteId} not found.");

            await _shipmentNoteRepository.DeleteAsync(note);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static ShipmentNoteDto MapToDto(ShipmentNote n) => new()
        {
            Id = n.Id,
            ShipmentId = n.ShipmentId,
            NoteType = n.NoteType,
            Content = n.Content,
            AttachmentUrl = n.AttachmentUrl,
            AttachmentFileName = n.AttachmentFileName,
            IsInternal = n.IsInternal,
            CreatedAt = n.CreatedAt,
            CreatedBy = n.CreatedBy
        };
    }
}
