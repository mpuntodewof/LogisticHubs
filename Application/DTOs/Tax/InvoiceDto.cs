using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Tax
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string? TaxInvoiceNumber { get; set; }
        public string? ReferenceDocumentType { get; set; }
        public Guid? ReferenceDocumentId { get; set; }
        public string? ReferenceDocumentNumber { get; set; }
        public string? CounterpartyName { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class InvoiceDetailDto : InvoiceDto
    {
        public IList<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
        public string? PaymentTermName { get; set; }
        public string? Notes { get; set; }
        public DateTime? IssuedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    public class InvoiceItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? TaxRateName { get; set; }
        public decimal TaxRateValue { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class CreateInvoiceRequest
    {
        public Guid? PaymentTermId { get; set; }
        public string? TaxInvoiceNumber { get; set; }
        public string? CounterpartyName { get; set; }
        public string? ReferenceDocumentType { get; set; }
        public Guid? ReferenceDocumentId { get; set; }
        public string? ReferenceDocumentNumber { get; set; }
        public string? Notes { get; set; }
        public DateTime? DueDate { get; set; }

        [Required]
        public List<CreateInvoiceItemRequest> Items { get; set; } = new();
    }

    public class CreateInvoiceItemRequest
    {
        [Required]
        public Guid ProductVariantId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public string VariantName { get; set; } = string.Empty;

        [Required]
        public string Sku { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
    }

    public class AssignTaxInvoiceNumberRequest
    {
        [Required]
        public string TaxInvoiceNumber { get; set; } = string.Empty;
    }
}
