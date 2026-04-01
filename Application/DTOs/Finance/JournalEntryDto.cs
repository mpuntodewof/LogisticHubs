using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Finance
{
    public class JournalEntryDto
    {
        public Guid Id { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public string? Description { get; set; }
        public string? Reference { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class JournalEntryDetailDto : JournalEntryDto
    {
        public IList<JournalEntryLineDto> Lines { get; set; } = new List<JournalEntryLineDto>();
        public string? ReferenceDocumentType { get; set; }
        public Guid? ReferenceDocumentId { get; set; }
        public DateTime? PostedAt { get; set; }
        public DateTime? VoidedAt { get; set; }
        public string? VoidReason { get; set; }
    }

    public class JournalEntryLineDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
    }

    public class CreateJournalEntryRequest
    {
        [Required]
        public DateTime EntryDate { get; set; }

        public string? Description { get; set; }

        public string? Reference { get; set; }

        [Required]
        public List<CreateJournalEntryLineRequest> Lines { get; set; } = new();
    }

    public class CreateJournalEntryLineRequest
    {
        [Required]
        public Guid AccountId { get; set; }

        public string? Description { get; set; }

        public decimal DebitAmount { get; set; }

        public decimal CreditAmount { get; set; }
    }

    public class VoidJournalEntryRequest
    {
        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}
