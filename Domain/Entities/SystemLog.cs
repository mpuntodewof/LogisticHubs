using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class SystemLog : BaseEntity
    {
        [Required, MaxLength(50)]
        public string Level { get; set; } = "Information";
        [Required, MaxLength(4000)]
        public string Message { get; set; } = string.Empty;
        [MaxLength(8000)]
        public string? Exception { get; set; }
        [MaxLength(200)]
        public string? Source { get; set; }
        [MaxLength(500)]
        public string? RequestPath { get; set; }
        [MaxLength(10)]
        public string? RequestMethod { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? UserId { get; set; }
        [MaxLength(8000)]
        public string? AdditionalDataJson { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
