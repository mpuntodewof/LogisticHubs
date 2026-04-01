using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Category : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public Guid? ParentCategoryId { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid TenantId { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public Category? ParentCategory { get; set; }
        public ICollection<Category> ChildCategories { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
