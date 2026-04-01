namespace BlazorApp.Client.Models
{
    // ── Suppliers ────────────────────────────────────────────────────────────

    public class SupplierDto
    {
        public Guid Id { get; set; }
        public string SupplierCode { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSupplierRequest
    {
        public string CompanyName { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }

    // ── Purchase Orders ──────────────────────────────────────────────────────

    public class PurchaseOrderDto
    {
        public Guid Id { get; set; }
        public string PoNumber { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ── Goods Receipts ───────────────────────────────────────────────────────

    public class GoodsReceiptDto
    {
        public Guid Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public string PoNumber { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ReceivedDate { get; set; }
    }
}
