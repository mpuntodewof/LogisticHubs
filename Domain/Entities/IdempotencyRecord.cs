using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class IdempotencyRecord
    {
        [Key]
        [MaxLength(100)]
        public string IdempotencyKey { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Endpoint { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public string? ResponseBody { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }
    }
}
