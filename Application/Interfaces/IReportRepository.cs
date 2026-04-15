using Application.DTOs.Reports;

namespace Application.Interfaces
{
    public interface IReportRepository
    {
        // P&L
        Task<List<AccountSummaryLine>> GetAccountSummariesAsync(DateTime from, DateTime to, string accountType);
        Task<List<ChannelProfitLine>> GetChannelProfitBreakdownAsync(DateTime from, DateTime to);

        // Dashboard
        Task<StockHealthSummary> GetStockHealthAsync();
        Task<SalesPerformanceSummary> GetSalesPerformanceAsync();
        Task<FinanceSummary> GetFinanceSummaryAsync();
        Task<List<RecentImportSummary>> GetRecentImportsAsync(int count = 5);
        Task<List<TopProductDto>> GetTopProductsAsync(DateTime from, DateTime to, int count = 10);
        Task<List<LowStockAlertDto>> GetLowStockAlertsAsync(int count = 20);
    }
}
