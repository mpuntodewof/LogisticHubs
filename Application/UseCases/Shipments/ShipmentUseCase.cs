using Application.DTOs.Common;
using Application.DTOs.Shipments;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Shipments
{
    public class ShipmentUseCase
    {
        private readonly IShipmentRepository _shipmentRepository;

        public ShipmentUseCase(IShipmentRepository shipmentRepository)
        {
            _shipmentRepository = shipmentRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<ShipmentDto>> GetAllAsync()
        {
            var shipments = await _shipmentRepository.GetAllAsync();
            return shipments.Select(MapToDto);
        }

        public async Task<PagedResult<ShipmentDto>> GetPagedAsync(PagedRequest request)
        {
            var paged = await _shipmentRepository.GetPagedAsync(request);
            return new PagedResult<ShipmentDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<ShipmentDto?> GetByIdAsync(Guid id)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(id);
            return shipment == null ? null : MapToDto(shipment);
        }

        public async Task<ShipmentDto?> GetByTrackingNumberAsync(string trackingNumber)
        {
            var shipment = await _shipmentRepository.GetByTrackingNumberAsync(trackingNumber);
            return shipment == null ? null : MapToDto(shipment);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<ShipmentDto> CreateAsync(CreateShipmentRequest request)
        {
            var trackingNumber = await GenerateUniqueTrackingNumberAsync();

            var shipment = new Shipment
            {
                Id = Guid.NewGuid(),
                TrackingNumber = trackingNumber,
                OriginWarehouseId = request.OriginWarehouseId,
                DestinationAddress = request.DestinationAddress,
                Weight = request.Weight,
                Volume = request.Volume,
                Status = ShipmentStatus.Pending.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            var created = await _shipmentRepository.CreateAsync(shipment);
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateShipmentRequest request)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Shipment {id} not found.");

            if (request.DestinationAddress != null) shipment.DestinationAddress = request.DestinationAddress;
            if (request.Weight.HasValue) shipment.Weight = request.Weight.Value;
            if (request.Volume.HasValue) shipment.Volume = request.Volume.Value;
            if (request.Status.HasValue) shipment.Status = request.Status.Value.ToString();

            await _shipmentRepository.UpdateAsync(shipment);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Shipment {id} not found.");

            if (shipment.Status != ShipmentStatus.Pending.ToString() &&
                shipment.Status != ShipmentStatus.Cancelled.ToString())
                throw new InvalidOperationException("Only Pending or Cancelled shipments can be deleted.");

            await _shipmentRepository.DeleteAsync(shipment);
        }

        // ── Assign ───────────────────────────────────────────────────────────────

        public async Task AssignAsync(Guid shipmentId, AssignShipmentRequest request)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(shipmentId)
                ?? throw new KeyNotFoundException($"Shipment {shipmentId} not found.");

            if (shipment.Status == ShipmentStatus.Delivered.ToString() ||
                shipment.Status == ShipmentStatus.Cancelled.ToString())
                throw new InvalidOperationException("Cannot assign a delivered or cancelled shipment.");

            var assignment = new ShipmentAssignment
            {
                Id = Guid.NewGuid(),
                ShipmentId = shipmentId,
                DriverId = request.DriverId,
                VehicleId = request.VehicleId,
                AssignedAt = DateTime.UtcNow
            };

            await _shipmentRepository.CreateAssignmentAsync(assignment);

            shipment.Status = ShipmentStatus.Assigned.ToString();
            await _shipmentRepository.UpdateAsync(shipment);
        }

        // ── Tracking ─────────────────────────────────────────────────────────────

        public async Task<IEnumerable<TrackingEventDto>> GetTrackingAsync(Guid shipmentId)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(shipmentId)
                ?? throw new KeyNotFoundException($"Shipment {shipmentId} not found.");

            var events = await _shipmentRepository.GetTrackingEventsAsync(shipmentId);
            return events.Select(e => new TrackingEventDto
            {
                Id = e.Id,
                ShipmentId = e.ShipmentId,
                Status = e.Status,
                Location = e.Location,
                Notes = e.Notes,
                CreatedAt = e.CreatedAt
            });
        }

        public async Task<TrackingEventDto> AddTrackingEventAsync(Guid shipmentId, AddTrackingEventRequest request)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(shipmentId)
                ?? throw new KeyNotFoundException($"Shipment {shipmentId} not found.");

            var trackingEvent = new ShipmentTracking
            {
                Id = Guid.NewGuid(),
                ShipmentId = shipmentId,
                Status = request.Status.ToString(),
                Location = request.Location,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _shipmentRepository.AddTrackingEventAsync(trackingEvent);

            // Sync shipment status with the latest tracking event
            shipment.Status = request.Status.ToString();
            await _shipmentRepository.UpdateAsync(shipment);

            return new TrackingEventDto
            {
                Id = created.Id,
                ShipmentId = created.ShipmentId,
                Status = created.Status,
                Location = created.Location,
                Notes = created.Notes,
                CreatedAt = created.CreatedAt
            };
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private async Task<string> GenerateUniqueTrackingNumberAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            string trackingNumber;
            do
            {
                var suffix = new string(Enumerable.Range(0, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray());
                trackingNumber = $"LH-{suffix}";
            }
            while (await _shipmentRepository.TrackingNumberExistsAsync(trackingNumber));

            return trackingNumber;
        }

        private static ShipmentDto MapToDto(Shipment s) => new()
        {
            Id = s.Id,
            TrackingNumber = s.TrackingNumber,
            OriginWarehouseId = s.OriginWarehouseId,
            DestinationAddress = s.DestinationAddress,
            Weight = s.Weight,
            Volume = s.Volume,
            Status = s.Status,
            CreatedAt = s.CreatedAt
        };
    }
}
