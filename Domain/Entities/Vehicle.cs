using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Vehicle
    {
        [Key]
        public Guid Id { get; set; }

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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<ShipmentAssignment> ShipmentAssignments { get; set; } = new List<ShipmentAssignment>();
    }
}
