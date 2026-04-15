using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ChartOfAccountRepository : IChartOfAccountRepository
    {
        private readonly AppDbContext _context;

        public ChartOfAccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ChartOfAccount>> GetPagedAsync(PagedRequest request, string? accountType = null)
        {
            var query = _context.ChartOfAccounts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(accountType))
            {
                query = query.Where(a => a.AccountType == accountType);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(a => a.AccountCode.ToLower().Contains(search)
                    || a.Name.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "accountcode" => request.SortDescending ? query.OrderByDescending(a => a.AccountCode) : query.OrderBy(a => a.AccountCode),
                "name" => request.SortDescending ? query.OrderByDescending(a => a.Name) : query.OrderBy(a => a.Name),
                _ => query.OrderBy(a => a.AccountCode)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<ChartOfAccount?> GetByIdAsync(Guid id)
            => await _context.ChartOfAccounts.Include(a => a.ParentAccount).FirstOrDefaultAsync(a => a.Id == id);

        public async Task<ChartOfAccount?> GetByAccountCodeAsync(string accountCode)
            => await _context.ChartOfAccounts.FirstOrDefaultAsync(a => a.AccountCode == accountCode);

        public async Task<bool> AccountCodeExistsAsync(string accountCode)
            => await _context.ChartOfAccounts.AnyAsync(a => a.AccountCode == accountCode);

        public async Task<bool> HasPostedJournalLinesAsync(Guid accountId)
            => await _context.JournalEntryLines
                .AnyAsync(l => l.AccountId == accountId && l.JournalEntry.Status == "Posted");

        public async Task<ChartOfAccount> CreateAsync(ChartOfAccount account)
        {
            _context.ChartOfAccounts.Add(account);
            return account;
        }

        public async Task UpdateAsync(ChartOfAccount account)
        {
            _context.ChartOfAccounts.Update(account);
        }

        public async Task DeleteAsync(ChartOfAccount account)
        {
            _context.ChartOfAccounts.Remove(account);
        }
    }
}
