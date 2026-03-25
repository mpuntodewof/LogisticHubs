using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Driver
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public ICollection<ShipmentAssignment> ShipmentAssignments { get; set; } = new List<ShipmentAssignment>();
    }
}
