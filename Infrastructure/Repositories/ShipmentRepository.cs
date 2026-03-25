using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly AppDbContext _context;

        public ShipmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Shipment>> GetAllAsync()
            => await _context.Shipments.OrderByDescending(s => s.CreatedAt).ToListAsync();

        public async Task<Shipment?> GetByIdAsync(Guid id)
            => await _context.Shipments.FindAsync(id);

        public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber)
            => await _context.Shipments.FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber);

        public async Task<bool> TrackingNumberExistsAsync(string trackingNumber)
            => await _context.Shipments.AnyAsync(s => s.TrackingNumber == trackingNumber);

        public async Task<Shipment> CreateAsync(Shipment shipment)
        {
            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();
            return shipment;
        }

        public async Task UpdateAsync(Shipment shipment)
        {
            _context.Shipments.Update(shipment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Shipment shipment)
        {
            _context.Shipments.Remove(shipment);
            await _context.SaveChangesAsync();
        }

        public async Task<ShipmentAssignment> CreateAssignmentAsync(ShipmentAssignment assignment)
        {
            _context.ShipmentAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        public async Task<ShipmentAssignment?> GetActiveAssignmentAsync(Guid shipmentId)
            => await _context.ShipmentAssignments
                .Where(a => a.ShipmentId == shipmentId)
                .OrderByDescending(a => a.AssignedAt)
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<ShipmentTracking>> GetTrackingEventsAsync(Guid shipmentId)
            => await _context.ShipmentTrackings
                .Where(t => t.ShipmentId == shipmentId)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();

        public async Task<ShipmentTracking> AddTrackingEventAsync(ShipmentTracking trackingEvent)
        {
            _context.ShipmentTrackings.Add(trackingEvent);
            await _context.SaveChangesAsync();
            return trackingEvent;
        }
    }
}
