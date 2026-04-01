using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class JournalEntryLine : BaseEntity, ITenantScoped
    {
        public Guid JournalEntryId { get; set; }
        public Guid AccountId { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DebitAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditAmount { get; set; }

        public Guid TenantId { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public JournalEntry JournalEntry { get; set; } = null!;
        public ChartOfAccount Account { get; set; } = null!;
    }
}
