using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Shipment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string TrackingNumber { get; set; } = string.Empty;

        [Required]
        public Guid OriginWarehouseId { get; set; }

        [Required]
        [MaxLength(500)]
        public string DestinationAddress { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Weight { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Volume { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(OriginWarehouseId))]
        public Warehouse OriginWarehouse { get; set; } = null!;

        public ICollection<ShipmentAssignment> ShipmentAssignments { get; set; } = new List<ShipmentAssignment>();
        public ICollection<ShipmentTracking> TrackingHistory { get; set; } = new List<ShipmentTracking>();
    }
}
