using Application.DTOs.Common;
using Application.Interfaces;
using Application.UseCases.Finance;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace StockLedger.Unit.Tests.UseCases;

public class AutoJournalEntryUseCaseTests
{
    private readonly IJournalEntryRepository _journalEntryRepo = Substitute.For<IJournalEntryRepository>();
    private readonly IChartOfAccountRepository _accountRepo = Substitute.For<IChartOfAccountRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly AutoJournalEntryUseCase _sut;

    private readonly ChartOfAccount _cashAccount;
    private readonly ChartOfAccount _arAccount;
    private readonly ChartOfAccount _revenueAccount;
    private readonly ChartOfAccount _feeAccount;

    public AutoJournalEntryUseCaseTests()
    {
        _sut = new AutoJournalEntryUseCase(_journalEntryRepo, _accountRepo, _unitOfWork);

        _journalEntryRepo.CreateAsync(Arg.Any<JournalEntry>()).Returns(ci => ci.Arg<JournalEntry>());

        _cashAccount = new ChartOfAccount
        {
            Id = Guid.NewGuid(),
            AccountCode = "1110",
            Name = "Cash",
            AccountType = AccountType.Asset.ToString(),
            NormalBalance = "Debit"
        };

        _arAccount = new ChartOfAccount
        {
            Id = Guid.NewGuid(),
            AccountCode = "1130",
            Name = "Accounts Receivable",
            AccountType = AccountType.Asset.ToString(),
            NormalBalance = "Debit"
        };

        _revenueAccount = new ChartOfAccount
        {
            Id = Guid.NewGuid(),
            AccountCode = "4100",
            Name = "Sales Revenue",
            AccountType = AccountType.Revenue.ToString(),
            NormalBalance = "Credit"
        };

        _feeAccount = new ChartOfAccount
        {
            Id = Guid.NewGuid(),
            AccountCode = "6200",
            Name = "Platform Fee Expense",
            AccountType = AccountType.Expense.ToString(),
            NormalBalance = "Debit"
        };
    }

    private void SetupAccountMocks()
    {
        _accountRepo.GetByAccountCodeAsync("1110").Returns(_cashAccount);
        _accountRepo.GetByAccountCodeAsync("1130").Returns(_arAccount);
        _accountRepo.GetByAccountCodeAsync("4100").Returns(_revenueAccount);
        _accountRepo.GetByAccountCodeAsync("6200").Returns(_feeAccount);
    }

    private static Invoice CreatePaidInvoice(decimal grandTotal)
    {
        return new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = "INV-001",
            Status = InvoiceStatus.Paid.ToString(),
            PaidAt = DateTime.UtcNow,
            GrandTotal = grandTotal,
            InvoiceDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30)
        };
    }

    // ── Invoice Payment Tests ────────────────────────────────────────────

    [Fact]
    public async Task CreateForInvoicePaymentAsync_PaidInvoice_CreatesBalancedEntry()
    {
        SetupAccountMocks();
        var invoice = CreatePaidInvoice(1_000_000m);

        var result = await _sut.CreateForInvoicePaymentAsync(invoice);

        result.Should().NotBeNull();
        result!.TotalDebit.Should().Be(1_000_000m);
        result.TotalCredit.Should().Be(1_000_000m);
        result.TotalDebit.Should().Be(result.TotalCredit);
        result.Lines.Should().HaveCount(2);

        var debitLine = result.Lines.First(l => l.DebitAmount > 0);
        var creditLine = result.Lines.First(l => l.CreditAmount > 0);

        debitLine.AccountId.Should().Be(_cashAccount.Id);
        debitLine.DebitAmount.Should().Be(1_000_000m);
        creditLine.AccountId.Should().Be(_arAccount.Id);
        creditLine.CreditAmount.Should().Be(1_000_000m);

        await _journalEntryRepo.Received(1).CreateAsync(Arg.Any<JournalEntry>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateForInvoicePaymentAsync_UnpaidInvoice_ReturnsNull()
    {
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = "INV-002",
            Status = InvoiceStatus.Issued.ToString(),
            GrandTotal = 500_000m,
            InvoiceDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30)
        };

        var result = await _sut.CreateForInvoicePaymentAsync(invoice);

        result.Should().BeNull();
        await _journalEntryRepo.DidNotReceive().CreateAsync(Arg.Any<JournalEntry>());
    }

    [Fact]
    public async Task CreateForInvoicePaymentAsync_ZeroAmount_ReturnsNull()
    {
        var invoice = CreatePaidInvoice(0m);

        var result = await _sut.CreateForInvoicePaymentAsync(invoice);

        result.Should().BeNull();
        await _journalEntryRepo.DidNotReceive().CreateAsync(Arg.Any<JournalEntry>());
    }

    [Fact]
    public async Task CreateForInvoicePaymentAsync_MissingAccounts_ReturnsNull()
    {
        _accountRepo.GetByAccountCodeAsync("1110").ReturnsNull();
        _accountRepo.GetByAccountCodeAsync("1130").ReturnsNull();
        _accountRepo.GetByAccountCodeAsync(Arg.Is<string>(s => s != "1110" && s != "1130"))
            .ReturnsNull();
        _accountRepo.GetPagedAsync(Arg.Any<PagedRequest>(), Arg.Any<string>())
            .Returns(new PagedResult<ChartOfAccount>
            {
                Items = new List<ChartOfAccount>(),
                TotalCount = 0,
                Page = 1,
                PageSize = 100
            });

        var invoice = CreatePaidInvoice(1_000_000m);

        var result = await _sut.CreateForInvoicePaymentAsync(invoice);

        result.Should().BeNull();
        await _journalEntryRepo.DidNotReceive().CreateAsync(Arg.Any<JournalEntry>());
    }

    // ── Sales Batch Tests ────────────────────────────────────────────────

    [Fact]
    public async Task CreateForSalesBatchAsync_WithFees_CreatesThreeLineEntry()
    {
        SetupAccountMocks();
        var batchId = Guid.NewGuid();

        var result = await _sut.CreateForSalesBatchAsync(batchId, "Tokopedia", 5_000_000m, 500_000m);

        result.Should().NotBeNull();
        result!.Lines.Should().HaveCount(3);
        result.TotalDebit.Should().Be(5_000_000m);
        result.TotalCredit.Should().Be(5_000_000m);
        result.TotalDebit.Should().Be(result.TotalCredit);

        var arLine = result.Lines.First(l => l.AccountId == _arAccount.Id);
        var feeLine = result.Lines.First(l => l.AccountId == _feeAccount.Id);
        var revenueLine = result.Lines.First(l => l.AccountId == _revenueAccount.Id);

        arLine.DebitAmount.Should().Be(4_500_000m);
        feeLine.DebitAmount.Should().Be(500_000m);
        revenueLine.CreditAmount.Should().Be(5_000_000m);

        var totalDebits = result.Lines.Sum(l => l.DebitAmount);
        var totalCredits = result.Lines.Sum(l => l.CreditAmount);
        totalDebits.Should().Be(totalCredits);

        await _journalEntryRepo.Received(1).CreateAsync(Arg.Any<JournalEntry>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateForSalesBatchAsync_ZeroRevenue_ReturnsNull()
    {
        var batchId = Guid.NewGuid();

        var result = await _sut.CreateForSalesBatchAsync(batchId, "Shopee", 0m, 0m);

        result.Should().BeNull();
        await _journalEntryRepo.DidNotReceive().CreateAsync(Arg.Any<JournalEntry>());
    }
}
