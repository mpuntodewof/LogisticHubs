using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Supplier : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required, MaxLength(50)]
        public string SupplierCode { get; set; } = string.Empty;
        [Required, MaxLength(500)]
        public string CompanyName { get; set; } = string.Empty;
        [MaxLength(255)]
        public string? ContactName { get; set; }
        [MaxLength(255), EmailAddress]
        public string? Email { get; set; }
        [MaxLength(50)]
        public string? Phone { get; set; }
        [MaxLength(1000)]
        public string? Address { get; set; }
        [MaxLength(255)]
        public string? City { get; set; }
        [MaxLength(255)]
        public string? Province { get; set; }
        [MaxLength(20)]
        public string? PostalCode { get; set; }
        [MaxLength(100)]
        public string Country { get; set; } = "Indonesia";
        [MaxLength(50)]
        public string? TaxId { get; set; }
        public Guid? PaymentTermId { get; set; }
        [MaxLength(255)]
        public string? BankName { get; set; }
        [MaxLength(100)]
        public string? BankAccountNumber { get; set; }
        [MaxLength(255)]
        public string? BankAccountName { get; set; }
        [MaxLength(2000)]
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid TenantId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public PaymentTerm? PaymentTerm { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
