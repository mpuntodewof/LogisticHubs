using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class DeliveryZone : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required, MaxLength(4000)]
        public string CoverageAreas { get; set; } = "[]";

        [MaxLength(255)]
        public string? Province { get; set; }

        public int EstimatedDeliveryDays { get; set; }
        public int MaxDeliveryDays { get; set; }
        public bool IsActive { get; set; } = true;

        public Guid TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public ICollection<DeliveryRate> DeliveryRates { get; set; } = new List<DeliveryRate>();
    }
}
