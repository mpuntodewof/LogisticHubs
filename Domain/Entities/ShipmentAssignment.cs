using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ShipmentAssignment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ShipmentId { get; set; }

        [Required]
        public Guid VehicleId { get; set; }

        [Required]
        public Guid DriverId { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;

        [ForeignKey(nameof(VehicleId))]
        public Vehicle Vehicle { get; set; } = null!;

        [ForeignKey(nameof(DriverId))]
        public Driver Driver { get; set; } = null!;
    }
}
