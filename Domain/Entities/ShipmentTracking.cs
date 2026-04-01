using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class ShipmentTracking : BaseEntity, ITenantScoped
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Location { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public Guid TenantId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ShipmentId))]
        public Shipment Shipment { get; set; } = null!;
    }
}
