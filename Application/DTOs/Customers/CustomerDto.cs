using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Customers
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string CustomerType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? TaxId { get; set; }
        public Guid? CustomerGroupId { get; set; }
        public string? CustomerGroupName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CustomerDetailDto : CustomerDto
    {
        public IList<CustomerAddressDto> Addresses { get; set; } = new List<CustomerAddressDto>();
    }

    public class CreateCustomerRequest
    {
        [Required]
        public CustomerType CustomerType { get; set; }

        [Required, StringLength(500)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? CompanyName { get; set; }

        [StringLength(255), EmailAddress]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(50)]
        public string? TaxId { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }

        public Guid? CustomerGroupId { get; set; }
    }

    public class UpdateCustomerRequest
    {
        public CustomerType? CustomerType { get; set; }

        [StringLength(500)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? CompanyName { get; set; }

        [StringLength(255), EmailAddress]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(50)]
        public string? TaxId { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }

        public Guid? CustomerGroupId { get; set; }
        public bool? IsActive { get; set; }
    }
}
