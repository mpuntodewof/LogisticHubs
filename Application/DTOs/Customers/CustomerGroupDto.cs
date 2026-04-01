using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Customers
{
    public class CustomerGroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCustomerGroupRequest
    {
        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public decimal DiscountPercentage { get; set; }
    }

    public class UpdateCustomerGroupRequest
    {
        [StringLength(255)]
        public string? Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public decimal? DiscountPercentage { get; set; }
        public bool? IsActive { get; set; }
    }
}
