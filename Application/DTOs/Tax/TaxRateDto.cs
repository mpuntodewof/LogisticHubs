using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Tax
{
    public class TaxRateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string TaxType { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTaxRateRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public TaxType TaxType { get; set; }

        [Required]
        public decimal Rate { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }

    public class UpdateTaxRateRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Rate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
