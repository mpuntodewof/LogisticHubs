namespace BlazorApp.Client.Models
{
    public class ShipmentDto
    {
        public Guid Id { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public Guid OriginWarehouseId { get; set; }
        public string DestinationAddress { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public decimal Volume { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateShipmentRequest
    {
        public Guid OriginWarehouseId { get; set; }
        public string DestinationAddress { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public decimal Volume { get; set; }
    }

    public class TrackingEventDto
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AssignShipmentRequest
    {
        public Guid DriverId { get; set; }
        public Guid VehicleId { get; set; }
    }

    public class AddTrackingEventRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Notes { get; set; }
    }
}
