using Application.DTOs.Common;
using Application.DTOs.Finance;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Finance
{
    public class JournalEntryUseCase
    {
        private readonly IJournalEntryRepository _repository;
        private readonly IChartOfAccountRepository _accountRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly IUnitOfWork _unitOfWork;

        public JournalEntryUseCase(
            IJournalEntryRepository repository,
            IChartOfAccountRepository accountRepository,
            ITransactionManager transactionManager,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _accountRepository = accountRepository;
            _transactionManager = transactionManager;
            _unitOfWork = unitOfWork;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<JournalEntryDto>> GetPagedAsync(PagedRequest request, string? status = null)
        {
            var paged = await _repository.GetPagedAsync(request, status);
            return new PagedResult<JournalEntryDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<JournalEntryDetailDto?> GetByIdAsync(Guid id)
        {
            var entry = await _repository.GetDetailByIdAsync(id);
            return entry == null ? null : MapToDetailDto(entry);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<JournalEntryDetailDto> CreateAsync(CreateJournalEntryRequest request)
        {
            if (request.Lines == null || request.Lines.Count == 0)
                throw new InvalidOperationException("Journal entry must have at least one line.");

            var totalDebit = request.Lines.Sum(l => l.DebitAmount);
            var totalCredit = request.Lines.Sum(l => l.CreditAmount);

            if (totalDebit != totalCredit)
                throw new InvalidOperationException($"Total debits ({totalDebit}) must equal total credits ({totalCredit}).");

            // Validate all account IDs exist
            foreach (var line in request.Lines)
            {
                var account = await _accountRepository.GetByIdAsync(line.AccountId);
                if (account == null)
                    throw new KeyNotFoundException($"Account {line.AccountId} not found.");
            }

            await _transactionManager.BeginTransactionAsync();
            try
            {
                // Generate entry number inside transaction
                var entryNumber = await GenerateEntryNumberAsync(request.EntryDate);

                var entry = new JournalEntry
                {
                    Id = Guid.NewGuid(),
                    EntryNumber = entryNumber,
                    EntryDate = request.EntryDate,
                    Description = request.Description,
                    Reference = request.Reference,
                    Status = "Draft",
                    TotalDebit = totalDebit,
                    TotalCredit = totalCredit,
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var line in request.Lines)
                {
                    entry.Lines.Add(new JournalEntryLine
                    {
                        Id = Guid.NewGuid(),
                        JournalEntryId = entry.Id,
                        AccountId = line.AccountId,
                        Description = line.Description,
                        DebitAmount = line.DebitAmount,
                        CreditAmount = line.CreditAmount,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                var created = await _repository.CreateAsync(entry);

                await _unitOfWork.SaveChangesAsync();
                await _transactionManager.CommitAsync();

                // Reload with lines and accounts for the response
                var detail = await _repository.GetDetailByIdAsync(created.Id);
                return MapToDetailDto(detail!);
            }
            catch
            {
                await _transactionManager.RollbackAsync();
                throw;
            }
        }

        // ── Post ─────────────────────────────────────────────────────────────────

        public async Task PostAsync(Guid id)
        {
            var entry = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Journal entry {id} not found.");

            entry.Post();

            await _repository.UpdateAsync(entry);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Void ─────────────────────────────────────────────────────────────────

        public async Task VoidAsync(Guid id, VoidJournalEntryRequest request)
        {
            var entry = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Journal entry {id} not found.");

            entry.Void(request.Reason);

            await _repository.UpdateAsync(entry);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var entry = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Journal entry {id} not found.");

            entry.EnsureCanDelete();

            await _repository.DeleteAsync(entry);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private async Task<string> GenerateEntryNumberAsync(DateTime entryDate)
        {
            var prefix = $"JE-{entryDate:yyyyMMdd}-";
            var counter = 1;
            string entryNumber;

            do
            {
                entryNumber = $"{prefix}{counter:D4}";
                counter++;
            }
            while (await _repository.EntryNumberExistsAsync(entryNumber));

            return entryNumber;
        }

        private static JournalEntryDto MapToDto(JournalEntry e) => new()
        {
            Id = e.Id,
            EntryNumber = e.EntryNumber,
            EntryDate = e.EntryDate,
            Description = e.Description,
            Reference = e.Reference,
            Status = e.Status,
            TotalDebit = e.TotalDebit,
            TotalCredit = e.TotalCredit,
            CreatedAt = e.CreatedAt
        };

        private static JournalEntryDetailDto MapToDetailDto(JournalEntry e) => new()
        {
            Id = e.Id,
            EntryNumber = e.EntryNumber,
            EntryDate = e.EntryDate,
            Description = e.Description,
            Reference = e.Reference,
            Status = e.Status,
            TotalDebit = e.TotalDebit,
            TotalCredit = e.TotalCredit,
            CreatedAt = e.CreatedAt,
            ReferenceDocumentType = e.ReferenceDocumentType,
            ReferenceDocumentId = e.ReferenceDocumentId,
            PostedAt = e.PostedAt,
            VoidedAt = e.VoidedAt,
            VoidReason = e.VoidReason,
            Lines = e.Lines.Select(l => new JournalEntryLineDto
            {
                Id = l.Id,
                AccountId = l.AccountId,
                AccountCode = l.Account?.AccountCode ?? string.Empty,
                AccountName = l.Account?.Name ?? string.Empty,
                Description = l.Description,
                DebitAmount = l.DebitAmount,
                CreditAmount = l.CreditAmount
            }).ToList()
        };
    }
}
