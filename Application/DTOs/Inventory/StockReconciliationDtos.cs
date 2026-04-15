using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Inventory
{
    public class StockReconciliationRequest
    {
        [Required]
        public Guid WarehouseId { get; set; }
        [Required]
        public List<StockCountLine> Counts { get; set; } = new();
        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class StockCountLine
    {
        [Required]
        public Guid ProductVariantId { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int PhysicalCount { get; set; }
    }

    public class StockReconciliationResult
    {
        public int TotalItems { get; set; }
        public int AdjustedItems { get; set; }
        public int MatchedItems { get; set; }
        public List<StockVarianceLine> Variances { get; set; } = new();
    }

    public class StockVarianceLine
    {
        public Guid ProductVariantId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int SystemCount { get; set; }
        public int PhysicalCount { get; set; }
        public int Variance { get; set; }
    }
}
