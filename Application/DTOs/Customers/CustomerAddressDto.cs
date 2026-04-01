using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Customers
{
    public class CustomerAddressDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Label { get; set; } = string.Empty;
        public string AddressType { get; set; } = string.Empty;
        public string RecipientName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCustomerAddressRequest
    {
        [Required, StringLength(100)]
        public string Label { get; set; } = string.Empty;

        [Required]
        public AddressType AddressType { get; set; }

        [Required, StringLength(255)]
        public string RecipientName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Phone { get; set; }

        [Required, StringLength(500)]
        public string AddressLine1 { get; set; } = string.Empty;

        [StringLength(500)]
        public string? AddressLine2 { get; set; }

        [Required, StringLength(255)]
        public string City { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Province { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [StringLength(100)]
        public string Country { get; set; } = "Indonesia";

        public bool IsDefault { get; set; }
    }

    public class UpdateCustomerAddressRequest
    {
        [StringLength(100)]
        public string? Label { get; set; }

        public AddressType? AddressType { get; set; }

        [StringLength(255)]
        public string? RecipientName { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? AddressLine1 { get; set; }

        [StringLength(500)]
        public string? AddressLine2 { get; set; }

        [StringLength(255)]
        public string? City { get; set; }

        [StringLength(255)]
        public string? Province { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        public bool? IsDefault { get; set; }
    }
}
