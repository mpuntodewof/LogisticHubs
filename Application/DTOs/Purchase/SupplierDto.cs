using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Purchase
{
    public class SupplierDto
    {
        public Guid Id { get; set; }
        public string SupplierCode { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? TaxId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SupplierDetailDto : SupplierDto
    {
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string Country { get; set; } = "Indonesia";
        public Guid? PaymentTermId { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountName { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateSupplierRequest
    {
        [Required, StringLength(500)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? ContactName { get; set; }

        [StringLength(255), EmailAddress]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(1000)]
        public string? Address { get; set; }

        [StringLength(255)]
        public string? City { get; set; }

        [StringLength(255)]
        public string? Province { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(50)]
        public string? TaxId { get; set; }

        public Guid? PaymentTermId { get; set; }

        [StringLength(255)]
        public string? BankName { get; set; }

        [StringLength(100)]
        public string? BankAccountNumber { get; set; }

        [StringLength(255)]
        public string? BankAccountName { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }
    }

    public class UpdateSupplierRequest
    {
        [StringLength(500)]
        public string? CompanyName { get; set; }

        [StringLength(255)]
        public string? ContactName { get; set; }

        [StringLength(255), EmailAddress]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(1000)]
        public string? Address { get; set; }

        [StringLength(255)]
        public string? City { get; set; }

        [StringLength(255)]
        public string? Province { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(50)]
        public string? TaxId { get; set; }

        public Guid? PaymentTermId { get; set; }

        [StringLength(255)]
        public string? BankName { get; set; }

        [StringLength(100)]
        public string? BankAccountNumber { get; set; }

        [StringLength(255)]
        public string? BankAccountName { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }

        public bool? IsActive { get; set; }
    }
}
