using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class ChartOfAccount : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required]
        [MaxLength(20)]
        public string AccountCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string AccountType { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? AccountSubType { get; set; }

        public Guid? ParentAccountId { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsSystemAccount { get; set; }

        [Required]
        [MaxLength(10)]
        public string NormalBalance { get; set; } = "Debit";

        public Guid TenantId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public ChartOfAccount? ParentAccount { get; set; }
        public ICollection<ChartOfAccount> ChildAccounts { get; set; } = new List<ChartOfAccount>();
        public ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
    }
}
