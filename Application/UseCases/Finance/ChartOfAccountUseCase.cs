using Application.DTOs.Common;
using Application.DTOs.Finance;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Finance
{
    public class ChartOfAccountUseCase
    {
        private readonly IChartOfAccountRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ChartOfAccountUseCase(IChartOfAccountRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<ChartOfAccountDto>> GetPagedAsync(PagedRequest request, string? accountType = null)
        {
            var paged = await _repository.GetPagedAsync(request, accountType);
            return new PagedResult<ChartOfAccountDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<ChartOfAccountDto?> GetByIdAsync(Guid id)
        {
            var account = await _repository.GetByIdAsync(id);
            return account == null ? null : MapToDto(account);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<ChartOfAccountDto> CreateAsync(CreateChartOfAccountRequest request)
        {
            if (await _repository.AccountCodeExistsAsync(request.AccountCode))
                throw new InvalidOperationException($"Account code '{request.AccountCode}' already exists.");

            if (request.ParentAccountId.HasValue)
            {
                _ = await _repository.GetByIdAsync(request.ParentAccountId.Value)
                    ?? throw new KeyNotFoundException($"Parent account {request.ParentAccountId.Value} not found.");
            }

            var normalBalance = request.NormalBalance;
            if (string.IsNullOrWhiteSpace(normalBalance))
            {
                normalBalance = request.AccountType switch
                {
                    AccountType.Asset or AccountType.Expense => "Debit",
                    AccountType.Liability or AccountType.Equity or AccountType.Revenue => "Credit",
                    _ => "Debit"
                };
            }

            var account = new ChartOfAccount
            {
                Id = Guid.NewGuid(),
                AccountCode = request.AccountCode,
                Name = request.Name,
                Description = request.Description,
                AccountType = request.AccountType.ToString(),
                AccountSubType = request.AccountSubType,
                ParentAccountId = request.ParentAccountId,
                NormalBalance = normalBalance,
                IsActive = true,
                IsSystemAccount = false,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(account);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateChartOfAccountRequest request)
        {
            var account = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Chart of account {id} not found.");

            if (request.Name != null) account.Name = request.Name;
            if (request.Description != null) account.Description = request.Description;
            if (request.AccountSubType != null) account.AccountSubType = request.AccountSubType;
            if (request.ParentAccountId.HasValue) account.ParentAccountId = request.ParentAccountId;
            if (request.IsActive.HasValue) account.IsActive = request.IsActive.Value;

            await _repository.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var account = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Chart of account {id} not found.");

            if (account.IsSystemAccount)
                throw new InvalidOperationException("Cannot delete a system account.");

            if (await _repository.HasPostedJournalLinesAsync(id))
                throw new InvalidOperationException("Cannot delete an account that has posted journal entries.");

            await _repository.DeleteAsync(account);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static ChartOfAccountDto MapToDto(ChartOfAccount a) => new()
        {
            Id = a.Id,
            AccountCode = a.AccountCode,
            Name = a.Name,
            Description = a.Description,
            AccountType = a.AccountType,
            AccountSubType = a.AccountSubType,
            ParentAccountId = a.ParentAccountId,
            ParentAccountName = a.ParentAccount?.Name,
            IsActive = a.IsActive,
            IsSystemAccount = a.IsSystemAccount,
            NormalBalance = a.NormalBalance,
            CreatedAt = a.CreatedAt
        };
    }
}
