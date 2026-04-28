using Application.DTOs.Common;
using Application.DTOs.Reports;
using Application.Interfaces;
using Domain.Enums;

namespace Application.UseCases.Reports
{
    public class ReportUseCase
    {
        private readonly IReportRepository _reportRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public ReportUseCase(IReportRepository reportRepository, IInvoiceRepository invoiceRepository)
        {
            _reportRepository = reportRepository;
            _invoiceRepository = invoiceRepository;
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

        public async Task<ProductMarginReport> GetProductMarginReportAsync(DateTime from, DateTime to)
        {
            var products = await _reportRepository.GetProductMarginsAsync(from, to);
            var byChannel = await _reportRepository.GetProductChannelMarginsAsync(from, to);

            foreach (var p in products)
            {
                p.NetMargin = p.Revenue - p.TotalCost - p.PlatformFees;
                p.NetMarginPercent = p.Revenue == 0 ? 0 : Math.Round(p.NetMargin / p.Revenue * 100, 2);
            }
            foreach (var pc in byChannel)
            {
                pc.NetMargin = pc.Revenue - pc.TotalCost - pc.PlatformFees;
                pc.NetMarginPercent = pc.Revenue == 0 ? 0 : Math.Round(pc.NetMargin / pc.Revenue * 100, 2);
            }

            // Sort: worst margin first per Q4 default — that's the actionable view.
            products = products.OrderBy(p => p.NetMarginPercent).ToList();
            byChannel = byChannel.OrderBy(pc => pc.NetMarginPercent).ToList();

            var totalRevenue = products.Sum(p => p.Revenue);
            var totalCogs = products.Sum(p => p.TotalCost);
            var totalFees = products.Sum(p => p.PlatformFees);
            var netMargin = totalRevenue - totalCogs - totalFees;

            return new ProductMarginReport
            {
                FromDate = from,
                ToDate = to,
                TotalRevenue = totalRevenue,
                TotalCogs = totalCogs,
                TotalPlatformFees = totalFees,
                TotalNetMargin = netMargin,
                NetMarginPercent = totalRevenue == 0 ? 0 : Math.Round(netMargin / totalRevenue * 100, 2),
                Products = products,
                ProductByChannel = byChannel
            };
        }

        public async Task<PpnSummaryReport> GetPpnSummaryAsync(int year, int month)
        {
            var invoices = await _reportRepository.GetPpnOutputInvoicesAsync(year, month);

            // Per-rate grouping. Effective rate = round(Ppn / Dpp * 100, 2).
            // Invoices with Dpp == 0 (rare — fully discounted, exempt) bucketed under "0%".
            var groups = invoices
                .GroupBy(i => i.Dpp == 0 ? 0m : Math.Round(i.PpnAmount / i.Dpp * 100m, 2))
                .Select(g => new PpnRateGroup
                {
                    RatePercent = g.Key,
                    TaxRateCode = $"PPN-{g.Key:0.##}",
                    TaxRateName = g.Key == 0m ? "PPN 0% / Exempt" : $"PPN {g.Key:0.##}%",
                    InvoiceCount = g.Count(),
                    Dpp = g.Sum(i => i.Dpp),
                    PpnAmount = g.Sum(i => i.PpnAmount)
                })
                .OrderByDescending(g => g.RatePercent)
                .ToList();

            var totalDpp = invoices.Sum(i => i.Dpp);
            var totalOutput = invoices.Sum(i => i.PpnAmount);

            return new PpnSummaryReport
            {
                Year = year,
                Month = month,
                TotalDpp = totalDpp,
                TotalPpnOutput = totalOutput,
                TotalPpnInput = 0m,                 // PurchaseInvoice not yet built (tracker 2.13)
                NetPpnPayable = totalOutput,        // = Output - 0
                InputAvailable = false,
                RateGroups = groups,
                Invoices = invoices
            };
        }

        public async Task<FinanceDashboardSummary> GetFinanceDashboardAsync()
        {
            var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var pnl = await GetProfitAndLossAsync(monthStart, DateTime.UtcNow);
            var finance = await _reportRepository.GetFinanceSummaryAsync();

            // Pull the 5 most recent invoices across all statuses.
            var recent = await _invoiceRepository.GetPagedAsync(
                new PagedRequest { Page = 1, PageSize = 5, SortBy = "InvoiceDate", SortDescending = true });

            return new FinanceDashboardSummary
            {
                Finance = finance,
                MonthToDateProfitAndLoss = pnl,
                ChannelBreakdown = pnl.ChannelBreakdown,
                RecentInvoices = recent.Items.Select(i => new RecentInvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    CounterpartyName = i.CounterpartyName,
                    Status = i.Status,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    GrandTotal = i.GrandTotal
                }).ToList()
            };
        }
    }
}
