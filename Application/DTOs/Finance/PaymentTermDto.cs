using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Finance
{
    public class PaymentTermDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DueDays { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePaymentTermRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int DueDays { get; set; }
    }

    public class UpdatePaymentTermRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? DueDays { get; set; }
        public bool? IsActive { get; set; }
    }
}
