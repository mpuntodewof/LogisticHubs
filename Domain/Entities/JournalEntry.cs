using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
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

        // ── Domain behavior ──────────────────────────────────────────────────

        public bool IsDraft => Status == JournalEntryStatus.Draft.ToString();
        public bool IsPosted => Status == JournalEntryStatus.Posted.ToString();
        public bool IsVoided => Status == JournalEntryStatus.Voided.ToString();

        public void Post()
        {
            if (!IsDraft)
                throw new InvalidOperationException("Only draft journal entries can be posted.");
            Status = JournalEntryStatus.Posted.ToString();
            PostedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Void(string? reason)
        {
            if (!IsPosted)
                throw new InvalidOperationException("Only posted journal entries can be voided.");
            Status = JournalEntryStatus.Voided.ToString();
            VoidedAt = DateTime.UtcNow;
            VoidReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }

        public void EnsureCanDelete()
        {
            if (!IsDraft)
                throw new InvalidOperationException("Only draft journal entries can be deleted.");
        }

        public void EnsureCanUpdate()
        {
            if (!IsDraft)
                throw new InvalidOperationException("Only draft journal entries can be modified.");
        }
    }
}
