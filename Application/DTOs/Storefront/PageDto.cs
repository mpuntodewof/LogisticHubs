using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Storefront
{
    public class PageDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string Status { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PageDetailDto : PageDto
    {
        public string? Content { get; set; }
    }

    public class CreatePageRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Content { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public PageStatus Status { get; set; } = PageStatus.Draft;
        public int SortOrder { get; set; }
    }

    public class UpdatePageRequest
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public PageStatus? Status { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }
    }
}
