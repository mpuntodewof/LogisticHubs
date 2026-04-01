using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class ShipmentAssignment : BaseEntity, ITenantScoped
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [Required]
        public Guid VehicleId { get; set; }

        [Required]
        public Guid DriverId { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public Guid TenantId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [ForeignKey(nameof(VehicleId))]
        public Vehicle Vehicle { get; set; } = null!;

        [ForeignKey(nameof(DriverId))]
        public Driver Driver { get; set; } = null!;
    }
}
