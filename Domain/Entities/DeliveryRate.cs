using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class DeliveryRate : BaseEntity, ITenantScoped, ISoftDeletable
    {
        public Guid DeliveryZoneId { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string RateType { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal FlatRateAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PerKgRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal MinWeight { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal MaxWeight { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal WeightRangeRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinOrderAmountForFree { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public DeliveryZone DeliveryZone { get; set; } = null!;
    }
}
