using Domain.Entities;

namespace Application.Interfaces
{
    public interface IShipmentRepository
    {
        Task<IEnumerable<Shipment>> GetAllAsync();
        Task<Shipment?> GetByIdAsync(Guid id);
        Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber);
        Task<Shipment> CreateAsync(Shipment shipment);
        Task UpdateAsync(Shipment shipment);
        Task DeleteAsync(Shipment shipment);
        Task<bool> TrackingNumberExistsAsync(string trackingNumber);

        Task<ShipmentAssignment> CreateAssignmentAsync(ShipmentAssignment assignment);
        Task<ShipmentAssignment?> GetActiveAssignmentAsync(Guid shipmentId);

        Task<IEnumerable<ShipmentTracking>> GetTrackingEventsAsync(Guid shipmentId);
        Task<ShipmentTracking> AddTrackingEventAsync(ShipmentTracking trackingEvent);
    }
}
