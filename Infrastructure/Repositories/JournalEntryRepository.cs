using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class JournalEntryRepository : IJournalEntryRepository
    {
        private readonly AppDbContext _context;

        public JournalEntryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<JournalEntry>> GetPagedAsync(PagedRequest request, string? status = null)
        {
            var query = _context.JournalEntries.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(e => e.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(e => e.EntryNumber.ToLower().Contains(search)
                    || (e.Description != null && e.Description.ToLower().Contains(search))
                    || (e.Reference != null && e.Reference.ToLower().Contains(search)));
            }

            query = request.SortBy?.ToLower() switch
            {
                "entrynumber" => request.SortDescending ? query.OrderByDescending(e => e.EntryNumber) : query.OrderBy(e => e.EntryNumber),
                "date" => request.SortDescending ? query.OrderByDescending(e => e.EntryDate) : query.OrderBy(e => e.EntryDate),
                _ => query.OrderByDescending(e => e.EntryDate)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<JournalEntry?> GetByIdAsync(Guid id)
            => await _context.JournalEntries.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<JournalEntry?> GetDetailByIdAsync(Guid id)
            => await _context.JournalEntries
                .Include(e => e.Lines)
                    .ThenInclude(l => l.Account)
                .FirstOrDefaultAsync(e => e.Id == id);

        public async Task<bool> EntryNumberExistsAsync(string entryNumber)
            => await _context.JournalEntries.AnyAsync(e => e.EntryNumber == entryNumber);

        public async Task<JournalEntry> CreateAsync(JournalEntry entry)
        {
            _context.JournalEntries.Add(entry);
            return entry;
        }

        public async Task UpdateAsync(JournalEntry entry)
        {
            _context.JournalEntries.Update(entry);
        }

        public async Task DeleteAsync(JournalEntry entry)
        {
            _context.JournalEntries.Remove(entry);
        }
    }
}
