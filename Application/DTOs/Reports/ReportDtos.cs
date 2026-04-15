namespace Application.DTOs.Reports
{
    // ── P&L Report ───────────────────────────────────────────────────────
    public class ProfitAndLossReport
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCogs { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal GrossProfitMargin { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public decimal NetProfitMargin { get; set; }
        public List<AccountSummaryLine> RevenueAccounts { get; set; } = new();
        public List<AccountSummaryLine> ExpenseAccounts { get; set; } = new();
        public List<ChannelProfitLine> ChannelBreakdown { get; set; } = new();
    }

    public class AccountSummaryLine
    {
        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class ChannelProfitLine
    {
        public Guid ChannelId { get; set; }
        public string ChannelName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal PlatformFees { get; set; }
        public decimal NetRevenue { get; set; }
        public int OrderCount { get; set; }
        public int UnitsSold { get; set; }
    }

    // ── Dashboard ────────────────────────────────────────────────────────
    public class DashboardSummary
    {
        public StockHealthSummary StockHealth { get; set; } = new();
        public SalesPerformanceSummary SalesPerformance { get; set; } = new();
        public FinanceSummary Finance { get; set; } = new();
        public List<RecentImportSummary> RecentImports { get; set; } = new();
        public List<TopProductDto> TopProducts { get; set; } = new();
        public List<LowStockAlertDto> LowStockAlerts { get; set; } = new();
    }

    public class StockHealthSummary
    {
        public int TotalSkus { get; set; }
        public int TotalUnitsInStock { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int WarehouseCount { get; set; }
    }

    public class SalesPerformanceSummary
    {
        public int TodayUnitsSold { get; set; }
        public int ThisWeekUnitsSold { get; set; }
        public int ThisMonthUnitsSold { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public int TodayOrderCount { get; set; }
        public int ThisMonthOrderCount { get; set; }
    }

    public class FinanceSummary
    {
        public decimal TotalReceivables { get; set; }
        public int OutstandingInvoices { get; set; }
        public int OverdueInvoices { get; set; }
        public decimal OverdueAmount { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal MonthlyExpenses { get; set; }
    }

    public class RecentImportSummary
    {
        public Guid BatchId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TotalRows { get; set; }
        public int SuccessRows { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TopProductDto
    {
        public Guid ProductVariantId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class LowStockAlertDto
    {
        public Guid WarehouseStockId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int QuantityOnHand { get; set; }
        public int? ReorderPoint { get; set; }
    }
}
