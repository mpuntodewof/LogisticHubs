using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities
{
    public class Invoice : BaseEntity, ITenantScoped, ISoftDeletable
    {
        private static readonly Dictionary<string, HashSet<string>> ValidTransitions = new()
        {
            [InvoiceStatus.Draft.ToString()] = [InvoiceStatus.Issued.ToString(), InvoiceStatus.Cancelled.ToString()],
            [InvoiceStatus.Issued.ToString()] = [InvoiceStatus.Paid.ToString(), InvoiceStatus.Cancelled.ToString()],
            [InvoiceStatus.Paid.ToString()] = [],
            [InvoiceStatus.Cancelled.ToString()] = []
        };

        [Required]
        [MaxLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? TaxInvoiceNumber { get; set; }

        [MaxLength(50)]
        public string? ReferenceDocumentType { get; set; }
        public Guid? ReferenceDocumentId { get; set; }
        [MaxLength(100)]
        public string? ReferenceDocumentNumber { get; set; }

        [MaxLength(255)]
        public string? CounterpartyName { get; set; }

        public Guid? PaymentTermId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxableAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GrandTotal { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        public DateTime? IssuedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        [MaxLength(1000)]
        public string? CancellationReason { get; set; }

        public Guid TenantId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        // Navigation
        public Tenant Tenant { get; set; } = null!;
        public PaymentTerm? PaymentTerm { get; set; }
        public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        // ── Domain behavior ──────────────────────────────────────────────────

        public bool IsDraft => Status == InvoiceStatus.Draft.ToString();
        public bool IsIssued => Status == InvoiceStatus.Issued.ToString();
        public bool IsPaid => Status == InvoiceStatus.Paid.ToString();
        public bool IsCancelled => Status == InvoiceStatus.Cancelled.ToString();

        public void Issue()
        {
            EnsureTransition(InvoiceStatus.Issued);
            Status = InvoiceStatus.Issued.ToString();
            IssuedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkPaid()
        {
            EnsureTransition(InvoiceStatus.Paid);
            Status = InvoiceStatus.Paid.ToString();
            PaidAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel(string? reason)
        {
            EnsureTransition(InvoiceStatus.Cancelled);
            Status = InvoiceStatus.Cancelled.ToString();
            CancelledAt = DateTime.UtcNow;
            CancellationReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }

        public void EnsureCanDelete()
        {
            if (!IsDraft)
                throw new InvalidOperationException("Only draft invoices can be deleted.");
        }

        public void EnsureCanUpdate()
        {
            if (!IsDraft)
                throw new InvalidOperationException("Only draft invoices can be modified.");
        }

        private void EnsureTransition(InvoiceStatus target)
        {
            var targetStr = target.ToString();
            if (!ValidTransitions.TryGetValue(Status, out var allowed) || !allowed.Contains(targetStr))
                throw new InvalidOperationException($"Cannot transition invoice from '{Status}' to '{targetStr}'.");
        }
    }
}
