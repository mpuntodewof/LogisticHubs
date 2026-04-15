using Application.DTOs.Reports;
using Application.Interfaces;
using Domain.Enums;

namespace Application.UseCases.Reports
{
    public class ReportUseCase
    {
        private readonly IReportRepository _reportRepository;

        public ReportUseCase(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<ProfitAndLossReport> GetProfitAndLossAsync(DateTime from, DateTime to)
        {
            var revenueAccounts = await _reportRepository.GetAccountSummariesAsync(from, to, AccountType.Revenue.ToString());
            var expenseAccounts = await _reportRepository.GetAccountSummariesAsync(from, to, AccountType.Expense.ToString());
            var channelBreakdown = await _reportRepository.GetChannelProfitBreakdownAsync(from, to);

            var totalRevenue = revenueAccounts.Sum(a => a.Amount);
            var totalExpenses = expenseAccounts.Sum(a => a.Amount);

            // COGS = expense accounts with sub-type containing "cost" or code starting with 5
            var cogsAccounts = expenseAccounts.Where(a =>
                a.AccountCode.StartsWith("5") ||
                a.AccountName.Contains("Cost", StringComparison.OrdinalIgnoreCase) ||
                a.AccountName.Contains("COGS", StringComparison.OrdinalIgnoreCase) ||
                a.AccountName.Contains("HPP", StringComparison.OrdinalIgnoreCase)).ToList();

            var totalCogs = cogsAccounts.Sum(a => a.Amount);
            var grossProfit = totalRevenue - totalCogs;
            var netProfit = totalRevenue - totalExpenses;

            return new ProfitAndLossReport
            {
                FromDate = from,
                ToDate = to,
                TotalRevenue = totalRevenue,
                TotalCogs = totalCogs,
                GrossProfit = grossProfit,
                GrossProfitMargin = totalRevenue == 0 ? 0 : Math.Round(grossProfit / totalRevenue * 100, 2),
                TotalExpenses = totalExpenses,
                NetProfit = netProfit,
                NetProfitMargin = totalRevenue == 0 ? 0 : Math.Round(netProfit / totalRevenue * 100, 2),
                RevenueAccounts = revenueAccounts,
                ExpenseAccounts = expenseAccounts,
                ChannelBreakdown = channelBreakdown
            };
        }

        public async Task<DashboardSummary> GetDashboardAsync()
        {
            var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            return new DashboardSummary
            {
                StockHealth = await _reportRepository.GetStockHealthAsync(),
                SalesPerformance = await _reportRepository.GetSalesPerformanceAsync(),
                Finance = await _reportRepository.GetFinanceSummaryAsync(),
                RecentImports = await _reportRepository.GetRecentImportsAsync(5),
                TopProducts = await _reportRepository.GetTopProductsAsync(monthStart, DateTime.UtcNow, 10),
                LowStockAlerts = await _reportRepository.GetLowStockAlertsAsync(20)
            };
        }
    }
}
