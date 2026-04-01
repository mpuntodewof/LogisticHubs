using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Vehicle : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required]
        [MaxLength(20)]
        public string PlateNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string VehicleType { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CapacityWeight { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CapacityVolume { get; set; }

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
        public ICollection<ShipmentAssignment> ShipmentAssignments { get; set; } = new List<ShipmentAssignment>();
    }
}
