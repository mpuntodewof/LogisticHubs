using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IChartOfAccountRepository
    {
        Task<PagedResult<ChartOfAccount>> GetPagedAsync(PagedRequest request, string? accountType = null);
        Task<ChartOfAccount?> GetByIdAsync(Guid id);
        Task<ChartOfAccount?> GetByAccountCodeAsync(string accountCode);
        Task<bool> AccountCodeExistsAsync(string accountCode);
        Task<bool> HasPostedJournalLinesAsync(Guid accountId);
        Task<ChartOfAccount> CreateAsync(ChartOfAccount account);
        Task UpdateAsync(ChartOfAccount account);
        Task DeleteAsync(ChartOfAccount account);
    }
}
