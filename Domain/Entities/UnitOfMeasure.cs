using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class UnitOfMeasure : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Abbreviation { get; set; } = string.Empty;

        public Guid TenantId { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<UnitConversion> ConversionsFrom { get; set; } = new List<UnitConversion>();
        public ICollection<UnitConversion> ConversionsTo { get; set; } = new List<UnitConversion>();
    }
}
