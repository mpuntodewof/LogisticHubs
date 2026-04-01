using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Categories
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CategoryTreeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public IList<CategoryTreeDto> Children { get; set; } = new List<CategoryTreeDto>();
    }

    public class CreateCategoryRequest
    {
        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public Guid? ParentCategoryId { get; set; }
        public int SortOrder { get; set; }
    }

    public class UpdateCategoryRequest
    {
        [StringLength(255)]
        public string? Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public Guid? ParentCategoryId { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }
    }
}
