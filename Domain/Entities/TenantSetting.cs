using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class TenantSetting : BaseEntity, ITenantScoped
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
        public Guid TenantId { get; set; }

        public Tenant Tenant { get; set; } = null!;
    }
}
