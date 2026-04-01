namespace BlazorApp.Client.Models
{
    // ── Chart of Accounts ────────────────────────────────────────────────────

    public class ChartOfAccountDto
    {
        public Guid Id { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string NormalBalance { get; set; } = string.Empty;
    }

    public class CreateChartOfAccountRequest
    {
        public string AccountCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
    }

    // ── Journal Entries ──────────────────────────────────────────────────────

    public class JournalEntryDto
    {
        public Guid Id { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
    }

    public class CreateJournalEntryRequest
    {
        public DateTime EntryDate { get; set; }
        public string? Description { get; set; }
        public List<CreateJournalEntryLineRequest> Lines { get; set; } = new();
    }

    public class CreateJournalEntryLineRequest
    {
        public Guid AccountId { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string? Description { get; set; }
    }

    // ── Invoices ─────────────────────────────────────────────────────────────

    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public Guid? SalesOrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ── Tax Rates ────────────────────────────────────────────────────────────

    public class TaxRateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string TaxType { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateTaxRateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string TaxType { get; set; } = string.Empty;
        public decimal Rate { get; set; }
    }

    // ── Payment Terms ────────────────────────────────────────────────────────

    public class PaymentTermDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int DueDays { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreatePaymentTermRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int DueDays { get; set; }
    }
}
