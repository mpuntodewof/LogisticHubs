namespace BlazorApp.Client.Models
{
    // ── Sales Orders ─────────────────────────────────────────────────────────

    public class SalesOrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string OrderType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid? BranchId { get; set; }
        public string? BranchName { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime OrderDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateSalesOrderRequest
    {
        public Guid CustomerId { get; set; }
        public Guid? BranchId { get; set; }
        public string? Notes { get; set; }
        public List<CreateSalesOrderItemRequest> Items { get; set; } = new();
    }

    public class CreateSalesOrderItemRequest
    {
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountAmount { get; set; }
    }

    public class SalesOrderPaymentDto
    {
        public Guid Id { get; set; }
        public Guid SalesOrderId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateSalesOrderPaymentRequest
    {
        public Guid SalesOrderId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    // ── Customers ────────────────────────────────────────────────────────────

    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string CustomerType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCustomerRequest
    {
        public string CustomerType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }

    // ── Customer Groups ──────────────────────────────────────────────────────

    public class CustomerGroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCustomerGroupRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal DiscountPercentage { get; set; }
    }
}
