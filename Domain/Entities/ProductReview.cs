using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class ProductReview : BaseEntity, ITenantScoped, ISoftDeletable
    {
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? SalesOrderId { get; set; }

        public int Rating { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(4000)]
        public string? Comment { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public bool IsVerifiedPurchase { get; set; }

        [MaxLength(2000)]
        public string? AdminResponse { get; set; }

        public DateTime? AdminRespondedAt { get; set; }

        public Guid TenantId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public SalesOrder? SalesOrder { get; set; }
    }
}
