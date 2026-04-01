using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class CustomerAddress : BaseEntity, ITenantScoped
    {
        public Guid CustomerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Label { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string AddressType { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string RecipientName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Phone { get; set; }

        [Required]
        [MaxLength(500)]
        public string AddressLine1 { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? AddressLine2 { get; set; }

        [Required]
        [MaxLength(255)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Province { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = "Indonesia";

        public bool IsDefault { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
    }
}
