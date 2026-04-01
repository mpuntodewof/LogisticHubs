using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Branch : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Province { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        public Guid? WarehouseId { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid TenantId { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public Warehouse? Warehouse { get; set; }
        public ICollection<BranchUser> BranchUsers { get; set; } = new List<BranchUser>();
        public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
    }
}
