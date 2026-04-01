using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Sales
{
    public class SalesOrderItemDto
    {
        public Guid Id { get; set; }
        public Guid SalesOrderId { get; set; }
        public Guid ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class CreateSalesOrderItemRequest
    {
        public Guid ProductVariantId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public decimal? DiscountAmount { get; set; }
    }
}
