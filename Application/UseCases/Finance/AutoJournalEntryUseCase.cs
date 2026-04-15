using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Finance
{
    public class AutoJournalEntryUseCase
    {
        private readonly IJournalEntryRepository _journalEntryRepository;
        private readonly IChartOfAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AutoJournalEntryUseCase(
            IJournalEntryRepository journalEntryRepository,
            IChartOfAccountRepository accountRepository,
            IUnitOfWork unitOfWork)
        {
            _journalEntryRepository = journalEntryRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Auto-create journal entry when an invoice is paid.
        /// Debit: Cash/Bank, Credit: Accounts Receivable
        /// </summary>
        public async Task<JournalEntry?> CreateForInvoicePaymentAsync(Invoice invoice)
        {
            if (!invoice.IsPaid || invoice.GrandTotal == 0) return null;

            var cashAccount = await _accountRepository.GetByAccountCodeAsync("1110")
                ?? await FindAccountByTypeAndNameAsync(AccountType.Asset, "Cash");
            var arAccount = await _accountRepository.GetByAccountCodeAsync("1130")
                ?? await FindAccountByTypeAndNameAsync(AccountType.Asset, "Receivable");

            if (cashAccount == null || arAccount == null) return null;

            var entry = new JournalEntry
            {
                Id = Guid.NewGuid(),
                EntryNumber = $"AUTO-PAY-{invoice.InvoiceNumber}",
                EntryDate = invoice.PaidAt ?? DateTime.UtcNow,
                Description = $"Auto: Payment received for {invoice.InvoiceNumber}",
                Reference = invoice.InvoiceNumber,
                ReferenceDocumentType = "Invoice",
                ReferenceDocumentId = invoice.Id,
                Status = JournalEntryStatus.Posted.ToString(),
                TotalDebit = invoice.GrandTotal,
                TotalCredit = invoice.GrandTotal,
                PostedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            entry.Lines.Add(new JournalEntryLine
            {
                Id = Guid.NewGuid(),
                JournalEntryId = entry.Id,
                AccountId = cashAccount.Id,
                Description = $"Payment: {invoice.InvoiceNumber}",
                DebitAmount = invoice.GrandTotal,
                CreditAmount = 0,
                CreatedAt = DateTime.UtcNow
            });

            entry.Lines.Add(new JournalEntryLine
            {
                Id = Guid.NewGuid(),
                JournalEntryId = entry.Id,
                AccountId = arAccount.Id,
                Description = $"Payment: {invoice.InvoiceNumber}",
                DebitAmount = 0,
                CreditAmount = invoice.GrandTotal,
                CreatedAt = DateTime.UtcNow
            });

            await _journalEntryRepository.CreateAsync(entry);
            await _unitOfWork.SaveChangesAsync();
            return entry;
        }

        /// <summary>
        /// Auto-create journal entry for CSV import sales batch.
        /// Debit: Accounts Receivable / Cash, Credit: Revenue
        /// Debit: COGS, Credit: Inventory
        /// </summary>
        public async Task<JournalEntry?> CreateForSalesBatchAsync(
            Guid batchId, string channelName, decimal totalRevenue, decimal platformFees)
        {
            if (totalRevenue == 0) return null;

            var arAccount = await _accountRepository.GetByAccountCodeAsync("1130")
                ?? await FindAccountByTypeAndNameAsync(AccountType.Asset, "Receivable");
            var revenueAccount = await _accountRepository.GetByAccountCodeAsync("4100")
                ?? await FindAccountByTypeAndNameAsync(AccountType.Revenue, "Revenue");
            var feeAccount = await _accountRepository.GetByAccountCodeAsync("6200")
                ?? await FindAccountByTypeAndNameAsync(AccountType.Expense, "Fee");

            if (arAccount == null || revenueAccount == null) return null;

            var entry = new JournalEntry
            {
                Id = Guid.NewGuid(),
                EntryNumber = $"AUTO-SALE-{DateTime.UtcNow:yyyyMMdd-HHmmss}",
                EntryDate = DateTime.UtcNow,
                Description = $"Auto: Sales import from {channelName}",
                ReferenceDocumentType = "CsvImportBatch",
                ReferenceDocumentId = batchId,
                Status = JournalEntryStatus.Posted.ToString(),
                TotalDebit = totalRevenue,
                TotalCredit = totalRevenue,
                PostedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            entry.Lines.Add(new JournalEntryLine
            {
                Id = Guid.NewGuid(),
                JournalEntryId = entry.Id,
                AccountId = arAccount.Id,
                Description = $"Sales receivable: {channelName}",
                DebitAmount = totalRevenue - platformFees,
                CreditAmount = 0,
                CreatedAt = DateTime.UtcNow
            });

            if (platformFees > 0 && feeAccount != null)
            {
                entry.Lines.Add(new JournalEntryLine
                {
                    Id = Guid.NewGuid(),
                    JournalEntryId = entry.Id,
                    AccountId = feeAccount.Id,
                    Description = $"Platform fee: {channelName}",
                    DebitAmount = platformFees,
                    CreditAmount = 0,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                // No fee account, put full amount in AR
                entry.Lines.First().DebitAmount = totalRevenue;
            }

            entry.Lines.Add(new JournalEntryLine
            {
                Id = Guid.NewGuid(),
                JournalEntryId = entry.Id,
                AccountId = revenueAccount.Id,
                Description = $"Revenue: {channelName}",
                DebitAmount = 0,
                CreditAmount = totalRevenue,
                CreatedAt = DateTime.UtcNow
            });

            await _journalEntryRepository.CreateAsync(entry);
            await _unitOfWork.SaveChangesAsync();
            return entry;
        }

        private async Task<ChartOfAccount?> FindAccountByTypeAndNameAsync(AccountType type, string nameContains)
        {
            var paged = await _accountRepository.GetPagedAsync(
                new Application.DTOs.Common.PagedRequest { Page = 1, PageSize = 100 },
                type.ToString());
            return paged.Items.FirstOrDefault(a =>
                a.Name.Contains(nameContains, StringComparison.OrdinalIgnoreCase));
        }
    }
}
