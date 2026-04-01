using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Logistics
{
    public class ShipmentNoteDto
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public string NoteType { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? AttachmentUrl { get; set; }
        public string? AttachmentFileName { get; set; }
        public bool IsInternal { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
    }

    public class CreateShipmentNoteRequest
    {
        [Required]
        public ShipmentNoteType NoteType { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        public string? AttachmentUrl { get; set; }
        public string? AttachmentFileName { get; set; }

        public bool IsInternal { get; set; } = true;
    }
}
