using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class SystemSetting : BaseEntity
    {
        [Required, MaxLength(200)]
        public string Key { get; set; } = string.Empty;
        [Required, MaxLength(4000)]
        public string Value { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        [MaxLength(100)]
        public string? Group { get; set; }
        [MaxLength(50)]
        public string? ValueType { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
