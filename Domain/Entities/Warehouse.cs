using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Warehouse : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Location { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public Guid TenantId { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation properties
        public Tenant Tenant { get; set; } = null!;
        public ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
    }
}
