using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Storefront
{
    public class BannerDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }
        public string Position { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateBannerRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public string? LinkUrl { get; set; }

        [Required]
        public BannerPosition Position { get; set; }

        public int SortOrder { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateBannerRequest
    {
        public string? Title { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public BannerPosition? Position { get; set; }
        public int? SortOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
