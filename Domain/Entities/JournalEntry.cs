using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class JournalEntry : BaseEntity, ITenantScoped, ISoftDeletable
    {
        [Required]
        [MaxLength(50)]
        public string EntryNumber { get; set; } = string.Empty;

        public DateTime EntryDate { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Reference { get; set; }

        [MaxLength(50)]
        public string? ReferenceDocumentType { get; set; }

        public Guid? ReferenceDocumentId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDebit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCredit { get; set; }

        public DateTime? PostedAt { get; set; }
        public DateTime? VoidedAt { get; set; }

        [MaxLength(1000)]
        public string? VoidReason { get; set; }

        public Guid TenantId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();
    }
}
