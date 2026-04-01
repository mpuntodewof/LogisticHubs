using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class CustomerGroup : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercentage { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid TenantId { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
