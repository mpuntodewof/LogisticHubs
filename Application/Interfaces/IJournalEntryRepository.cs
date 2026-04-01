using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IJournalEntryRepository
    {
        Task<PagedResult<JournalEntry>> GetPagedAsync(PagedRequest request, string? status = null);
        Task<JournalEntry?> GetByIdAsync(Guid id);
        Task<JournalEntry?> GetDetailByIdAsync(Guid id);
        Task<bool> EntryNumberExistsAsync(string entryNumber);
        Task<JournalEntry> CreateAsync(JournalEntry entry);
        Task UpdateAsync(JournalEntry entry);
        Task DeleteAsync(JournalEntry entry);
    }
}
