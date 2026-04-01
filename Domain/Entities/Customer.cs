using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Customer : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required]
        [MaxLength(50)]
        public string CustomerCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CustomerType { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? CompanyName { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(50)]
        public string? TaxId { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        public Guid? CustomerGroupId { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid TenantId { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public CustomerGroup? CustomerGroup { get; set; }
        public ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
        public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
        public ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
        public ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();
        public ICollection<LoyaltyMembership> LoyaltyMemberships { get; set; } = new List<LoyaltyMembership>();
        public ICollection<PromotionUsage> PromotionUsages { get; set; } = new List<PromotionUsage>();
    }
}
