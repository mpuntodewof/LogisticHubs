using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Tenant : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string CompanyName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
