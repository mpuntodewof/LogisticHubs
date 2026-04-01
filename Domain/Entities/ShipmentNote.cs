using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class ShipmentNote : BaseEntity, ITenantScoped
    {
        public Guid ShipmentId { get; set; }

        [Required, MaxLength(50)]
        public string NoteType { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? AttachmentUrl { get; set; }

        [MaxLength(255)]
        public string? AttachmentFileName { get; set; }

        public bool IsInternal { get; set; } = true;

        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public Shipment Shipment { get; set; } = null!;
    }
}
