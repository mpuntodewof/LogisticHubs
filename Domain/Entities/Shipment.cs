using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Shipment : BaseEntity, ITenantScoped, ISoftDeletable
    {
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

        public Guid TenantId { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation properties
        public Tenant Tenant { get; set; } = null!;

        [ForeignKey(nameof(OriginWarehouseId))]
        public Warehouse OriginWarehouse { get; set; } = null!;

        public ICollection<ShipmentAssignment> ShipmentAssignments { get; set; } = new List<ShipmentAssignment>();
        public ICollection<ShipmentTracking> TrackingHistory { get; set; } = new List<ShipmentTracking>();
        public ICollection<ShipmentNote> Notes { get; set; } = new List<ShipmentNote>();
    }
}
