using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class UnitConversion : BaseEntity, ITenantScoped
    {
        public Guid FromUnitId { get; set; }
        public Guid ToUnitId { get; set; }

        [Column(TypeName = "decimal(18,6)")]
        public decimal ConversionFactor { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public UnitOfMeasure FromUnit { get; set; } = null!;
        public UnitOfMeasure ToUnit { get; set; } = null!;
    }
}
