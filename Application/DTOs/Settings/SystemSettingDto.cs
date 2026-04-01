using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Settings
{
    public class SystemSettingDto
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Group { get; set; }
        public string? ValueType { get; set; }
        public bool IsReadOnly { get; set; }
    }

    public class UpdateSystemSettingRequest
    {
        [Required]
        public string Value { get; set; } = string.Empty;
    }
}
