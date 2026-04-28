namespace BlazorApp.Client.Models
{
    public class DashboardSummary
    {
        public StockHealthSummary StockHealth { get; set; } = new();
        public SalesPerformanceSummary SalesPerformance { get; set; } = new();
        public FinanceSummaryDto Finance { get; set; } = new();
        public List<RecentImportDto> RecentImports { get; set; } = new();
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

    public class FinanceSummaryDto
    {
        public decimal TotalReceivables { get; set; }
        public int OutstandingInvoices { get; set; }
        public int OverdueInvoices { get; set; }
        public decimal OverdueAmount { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal MonthlyExpenses { get; set; }
    }

    public class RecentImportDto
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
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class LowStockAlertDto
    {
        public string WarehouseName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int QuantityOnHand { get; set; }
        public int? ReorderPoint { get; set; }
    }

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
        public decimal Amount { get; set; }
    }

    public class ChannelProfitLine
    {
        public string ChannelName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal PlatformFees { get; set; }
        public decimal NetRevenue { get; set; }
        public int OrderCount { get; set; }
        public int UnitsSold { get; set; }
    }

    public class FinanceDashboardSummary
    {
        public FinanceSummaryDto Finance { get; set; } = new();
        public ProfitAndLossReport MonthToDateProfitAndLoss { get; set; } = new();
        public List<ChannelProfitLine> ChannelBreakdown { get; set; } = new();
        public List<RecentInvoiceDto> RecentInvoices { get; set; } = new();
    }

    public class RecentInvoiceDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string? CounterpartyName { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal GrandTotal { get; set; }
    }

    // Mirrors Application.DTOs.Reports.ProductMarginReport. Cost source is
    // ProductVariant.CostPrice "as of now" (not cost-at-time-of-sale) — UI surfaces
    // this caveat on the page.
    public class ProductMarginReport
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCogs { get; set; }
        public decimal TotalPlatformFees { get; set; }
        public decimal TotalNetMargin { get; set; }
        public decimal NetMarginPercent { get; set; }
        public List<ProductMarginLine> Products { get; set; } = new();
        public List<ProductChannelMarginLine> ProductByChannel { get; set; } = new();
    }

    public class ProductMarginLine
    {
        public Guid ProductVariantId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
        public decimal CostPrice { get; set; }
        public decimal TotalCost { get; set; }
        public decimal PlatformFees { get; set; }
        public decimal NetMargin { get; set; }
        public decimal NetMarginPercent { get; set; }
    }

    public class ProductChannelMarginLine
    {
        public Guid ProductVariantId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public Guid ChannelId { get; set; }
        public string ChannelName { get; set; } = string.Empty;
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
        public decimal CostPrice { get; set; }
        public decimal TotalCost { get; set; }
        public decimal PlatformFees { get; set; }
        public decimal NetMargin { get; set; }
        public decimal NetMarginPercent { get; set; }
    }

    // Mirrors Application.DTOs.Reports.PpnSummaryReport. Output-only in v1
    // (PurchaseInvoice not yet built — Input PPN tracking deferred per tracker 2.13).
    public class PpnSummaryReport
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalDpp { get; set; }
        public decimal TotalPpnOutput { get; set; }
        public decimal TotalPpnInput { get; set; }
        public decimal NetPpnPayable { get; set; }
        public bool InputAvailable { get; set; }
        public List<PpnRateGroup> RateGroups { get; set; } = new();
        public List<PpnInvoiceLine> Invoices { get; set; } = new();
    }

    public class PpnRateGroup
    {
        public string TaxRateCode { get; set; } = string.Empty;
        public string TaxRateName { get; set; } = string.Empty;
        public decimal RatePercent { get; set; }
        public int InvoiceCount { get; set; }
        public decimal Dpp { get; set; }
        public decimal PpnAmount { get; set; }
    }

    public class PpnInvoiceLine
    {
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string? TaxInvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? CounterpartyName { get; set; }
        public string? CounterpartyNPWP { get; set; }
        public decimal Dpp { get; set; }
        public decimal PpnAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class NotificationSummary
    {
        public int TotalUnread { get; set; }
        public List<NotificationDto> Notifications { get; set; } = new();
    }

    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
