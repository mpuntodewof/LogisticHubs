using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Finance
{
    public class ChartOfAccountDto
    {
        public Guid Id { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string AccountType { get; set; } = string.Empty;
        public string? AccountSubType { get; set; }
        public Guid? ParentAccountId { get; set; }
        public string? ParentAccountName { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystemAccount { get; set; }
        public string NormalBalance { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateChartOfAccountRequest
    {
        [Required, MaxLength(20)]
        public string AccountCode { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public AccountType AccountType { get; set; }

        public string? AccountSubType { get; set; }

        public Guid? ParentAccountId { get; set; }

        public string? NormalBalance { get; set; }
    }

    public class UpdateChartOfAccountRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? AccountSubType { get; set; }
        public Guid? ParentAccountId { get; set; }
        public bool? IsActive { get; set; }
    }
}
