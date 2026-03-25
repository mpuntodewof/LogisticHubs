using Domain.Enums;

namespace Application.DTOs.Shipments
{
    public class TrackingEventDto
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AddTrackingEventRequest
    {
        public ShipmentStatus Status { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
    }
}
