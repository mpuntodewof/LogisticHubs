using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Reporting
{
    public class DashboardWidgetDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string WidgetType { get; set; } = string.Empty;
        public string? DataSourceKey { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int SortOrder { get; set; }
        public bool IsVisible { get; set; }
        public Guid? UserId { get; set; }
    }

    public class DashboardWidgetDetailDto : DashboardWidgetDto
    {
        public string? ConfigJson { get; set; }
    }

    public class CreateDashboardWidgetRequest
    {
        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string WidgetType { get; set; } = string.Empty;

        [StringLength(200)]
        public string? DataSourceKey { get; set; }

        public string? ConfigJson { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; } = 1;
        public int Height { get; set; } = 1;
        public int SortOrder { get; set; }
        public bool IsVisible { get; set; } = true;
        public Guid? UserId { get; set; }
    }

    public class UpdateDashboardWidgetRequest
    {
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(50)]
        public string? WidgetType { get; set; }

        [StringLength(200)]
        public string? DataSourceKey { get; set; }

        public string? ConfigJson { get; set; }
        public int? PositionX { get; set; }
        public int? PositionY { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsVisible { get; set; }
        public Guid? UserId { get; set; }
    }
}
