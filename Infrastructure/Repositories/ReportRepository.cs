using Application.DTOs.Reports;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AccountSummaryLine>> GetAccountSummariesAsync(DateTime from, DateTime to, string accountType)
        {
            return await _context.Set<JournalEntryLine>()
                .Where(l => l.JournalEntry.Status == JournalEntryStatus.Posted.ToString()
                    && l.JournalEntry.EntryDate >= from
                    && l.JournalEntry.EntryDate <= to
                    && l.Account.AccountType == accountType)
                .GroupBy(l => new { l.Account.AccountCode, l.Account.Name, l.Account.AccountType, l.Account.NormalBalance })
                .Select(g => new AccountSummaryLine
                {
                    AccountCode = g.Key.AccountCode,
                    AccountName = g.Key.Name,
                    AccountType = g.Key.AccountType,
                    Amount = g.Key.NormalBalance == "Debit"
                        ? g.Sum(l => l.DebitAmount) - g.Sum(l => l.CreditAmount)
                        : g.Sum(l => l.CreditAmount) - g.Sum(l => l.DebitAmount)
                })
                .OrderBy(a => a.AccountCode)
                .ToListAsync();
        }

        public async Task<List<ChannelProfitLine>> GetChannelProfitBreakdownAsync(DateTime from, DateTime to)
        {
            return await _context.Set<CsvImportRow>()
                .Where(r => r.Status == ImportRowStatus.Matched.ToString()
                    && r.Batch.CreatedAt >= from
                    && r.Batch.CreatedAt <= to)
                .GroupBy(r => new { r.Batch.SalesChannelId, r.Batch.SalesChannel.Name })
                .Select(g => new ChannelProfitLine
                {
                    ChannelId = g.Key.SalesChannelId,
                    ChannelName = g.Key.Name,
                    Revenue = g.Sum(r => r.TotalPrice),
                    PlatformFees = g.Sum(r => r.PlatformFee),
                    NetRevenue = g.Sum(r => r.TotalPrice) - g.Sum(r => r.PlatformFee),
                    OrderCount = g.Select(r => r.OrderNumber).Distinct().Count(),
                    UnitsSold = g.Sum(r => r.Quantity)
                })
                .OrderByDescending(c => c.Revenue)
                .ToListAsync();
        }

        public async Task<StockHealthSummary> GetStockHealthAsync()
        {
            var stocks = _context.Set<WarehouseStock>();
            return new StockHealthSummary
            {
                TotalSkus = await stocks.Select(s => s.ProductVariantId).Distinct().CountAsync(),
                TotalUnitsInStock = await stocks.SumAsync(s => s.QuantityOnHand),
                LowStockCount = await stocks.CountAsync(s => s.ReorderPoint != null && s.QuantityOnHand <= s.ReorderPoint && s.QuantityOnHand > 0),
                OutOfStockCount = await stocks.CountAsync(s => s.QuantityOnHand == 0),
                WarehouseCount = await _context.Set<Warehouse>().CountAsync()
            };
        }

        public async Task<SalesPerformanceSummary> GetSalesPerformanceAsync()
        {
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var salesMovements = _context.Set<StockMovement>()
                .Where(m => m.Reason == StockMovementReason.Sale.ToString());

            var importRows = _context.Set<CsvImportRow>()
                .Where(r => r.Status == ImportRowStatus.Matched.ToString());

            return new SalesPerformanceSummary
            {
                TodayUnitsSold = await salesMovements.Where(m => m.CreatedAt >= today).SumAsync(m => m.Quantity),
                ThisWeekUnitsSold = await salesMovements.Where(m => m.CreatedAt >= weekStart).SumAsync(m => m.Quantity),
                ThisMonthUnitsSold = await salesMovements.Where(m => m.CreatedAt >= monthStart).SumAsync(m => m.Quantity),
                TodayRevenue = await importRows.Where(r => r.Batch.CreatedAt >= today).SumAsync(r => r.TotalPrice),
                ThisMonthRevenue = await importRows.Where(r => r.Batch.CreatedAt >= monthStart).SumAsync(r => r.TotalPrice),
                TodayOrderCount = await importRows.Where(r => r.Batch.CreatedAt >= today).Select(r => r.OrderNumber).Distinct().CountAsync(),
                ThisMonthOrderCount = await importRows.Where(r => r.Batch.CreatedAt >= monthStart).Select(r => r.OrderNumber).Distinct().CountAsync()
            };
        }

        public async Task<FinanceSummary> GetFinanceSummaryAsync()
        {
            var today = DateTime.UtcNow.Date;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var issuedInvoices = _context.Set<Invoice>()
                .Where(i => i.Status == InvoiceStatus.Issued.ToString());

            var postedEntries = _context.Set<JournalEntryLine>()
                .Where(l => l.JournalEntry.Status == JournalEntryStatus.Posted.ToString()
                    && l.JournalEntry.EntryDate >= monthStart
                    && l.JournalEntry.EntryDate <= today);

            return new FinanceSummary
            {
                TotalReceivables = await issuedInvoices.SumAsync(i => i.GrandTotal),
                OutstandingInvoices = await issuedInvoices.CountAsync(),
                OverdueInvoices = await issuedInvoices.CountAsync(i => i.DueDate < today),
                OverdueAmount = await issuedInvoices.Where(i => i.DueDate < today).SumAsync(i => i.GrandTotal),
                MonthlyRevenue = await postedEntries
                    .Where(l => l.Account.AccountType == AccountType.Revenue.ToString())
                    .SumAsync(l => l.CreditAmount - l.DebitAmount),
                MonthlyExpenses = await postedEntries
                    .Where(l => l.Account.AccountType == AccountType.Expense.ToString())
                    .SumAsync(l => l.DebitAmount - l.CreditAmount)
            };
        }

        public async Task<List<RecentImportSummary>> GetRecentImportsAsync(int count = 5)
        {
            return await _context.Set<CsvImportBatch>()
                .Include(b => b.SalesChannel)
                .OrderByDescending(b => b.CreatedAt)
                .Take(count)
                .Select(b => new RecentImportSummary
                {
                    BatchId = b.Id,
                    FileName = b.FileName,
                    ChannelName = b.SalesChannel.Name,
                    Status = b.Status,
                    TotalRows = b.TotalRows,
                    SuccessRows = b.SuccessRows,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<TopProductDto>> GetTopProductsAsync(DateTime from, DateTime to, int count = 10)
        {
            return await _context.Set<CsvImportRow>()
                .Where(r => r.Status == ImportRowStatus.Matched.ToString()
                    && r.MatchedProductVariantId != null
                    && r.Batch.CreatedAt >= from
                    && r.Batch.CreatedAt <= to)
                .GroupBy(r => new
                {
                    r.MatchedProductVariantId,
                    r.MatchedProductVariant!.Sku,
                    ProductName = r.MatchedProductVariant.Product.Name + " - " + r.MatchedProductVariant.Name
                })
                .Select(g => new TopProductDto
                {
                    ProductVariantId = g.Key.MatchedProductVariantId!.Value,
                    Sku = g.Key.Sku,
                    ProductName = g.Key.ProductName,
                    UnitsSold = g.Sum(r => r.Quantity),
                    Revenue = g.Sum(r => r.TotalPrice)
                })
                .OrderByDescending(p => p.UnitsSold)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<LowStockAlertDto>> GetLowStockAlertsAsync(int count = 20)
        {
            return await _context.Set<WarehouseStock>()
                .Include(s => s.Warehouse)
                .Include(s => s.ProductVariant)
                    .ThenInclude(v => v.Product)
                .Where(s => (s.ReorderPoint != null && s.QuantityOnHand <= s.ReorderPoint) || s.QuantityOnHand == 0)
                .OrderBy(s => s.QuantityOnHand)
                .Take(count)
                .Select(s => new LowStockAlertDto
                {
                    WarehouseStockId = s.Id,
                    WarehouseName = s.Warehouse.Name,
                    Sku = s.ProductVariant.Sku,
                    ProductName = s.ProductVariant.Product.Name + " - " + s.ProductVariant.Name,
                    QuantityOnHand = s.QuantityOnHand,
                    ReorderPoint = s.ReorderPoint
                })
                .ToListAsync();
        }
    }
}
