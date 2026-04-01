using System.ComponentModel.DataAnnotations;
using Application.DTOs.Customers;
using Domain.Enums;

namespace Application.DTOs.Sales
{
    public class SalesOrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string OrderType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
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

    public class SalesOrderDetailDto : SalesOrderDto
    {
        public IList<SalesOrderItemDto> Items { get; set; } = new List<SalesOrderItemDto>();
        public IList<SalesOrderPaymentDto> Payments { get; set; } = new List<SalesOrderPaymentDto>();
        public CustomerAddressDto? ShippingAddress { get; set; }
    }

    public class CreateSalesOrderRequest
    {
        [Required]
        public SalesOrderType OrderType { get; set; }

        public Guid? CustomerId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? ShippingAddressId { get; set; }

        [Required]
        public List<CreateSalesOrderItemRequest> Items { get; set; } = new();

        public decimal DiscountAmount { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }
    }

    public class UpdateSalesOrderRequest
    {
        public Guid? ShippingAddressId { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? ShippingCost { get; set; }

        [StringLength(2000)]
        public string? Notes { get; set; }
    }
}
