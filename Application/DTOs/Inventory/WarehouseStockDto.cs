namespace Application.DTOs.Inventory
{
    public class WarehouseStockDto
    {
        public Guid Id { get; set; }
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public Guid ProductVariantId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ProductVariantName { get; set; } = string.Empty;
        public int QuantityOnHand { get; set; }
        public int QuantityReserved { get; set; }
        public int QuantityAvailable { get; set; }
        public int? ReorderPoint { get; set; }
        public int? MaxStock { get; set; }
    }

    public class UpdateWarehouseStockRequest
    {
        public int? ReorderPoint { get; set; }
        public int? MaxStock { get; set; }
    }
}
