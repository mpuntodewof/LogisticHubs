using Application.DTOs.Reports;
using Application.Interfaces;
using Application.UseCases.Reports;
using FluentAssertions;
using NSubstitute;

namespace StockLedger.Unit.Tests.UseCases;

// PPN math lives in ReportUseCase, not the repository. The repo returns
// per-invoice rows; the use case derives effective tax rate, groups by it,
// computes totals, and flags Input as unavailable until PurchaseInvoice exists.
public class ReportUseCasePpnTests
{
    private readonly IReportRepository _reportRepo = Substitute.For<IReportRepository>();
    private readonly IInvoiceRepository _invoiceRepo = Substitute.For<IInvoiceRepository>();
    private readonly ReportUseCase _sut;

    public ReportUseCasePpnTests()
    {
        _sut = new ReportUseCase(_reportRepo, _invoiceRepo);
    }

    private static PpnInvoiceLine Inv(decimal dpp, decimal ppn, string status = "Issued") =>
        new()
        {
            InvoiceId = Guid.NewGuid(),
            InvoiceNumber = "INV-1",
            InvoiceDate = new DateTime(2026, 4, 15),
            Dpp = dpp,
            PpnAmount = ppn,
            GrandTotal = dpp + ppn,
            Status = status
        };

    [Fact]
    public async Task Sums_total_dpp_and_ppn_output_across_all_invoices()
    {
        var rows = new List<PpnInvoiceLine>
        {
            Inv(1_000_000m, 110_000m),
            Inv(500_000m, 55_000m),
            Inv(200_000m, 22_000m)
        };
        _reportRepo.GetPpnOutputInvoicesAsync(2026, 4).Returns(rows);

        var result = await _sut.GetPpnSummaryAsync(2026, 4);

        result.TotalDpp.Should().Be(1_700_000m);
        result.TotalPpnOutput.Should().Be(187_000m);
    }

    [Fact]
    public async Task Net_payable_equals_output_when_input_is_unavailable()
    {
        _reportRepo.GetPpnOutputInvoicesAsync(2026, 4).Returns(new List<PpnInvoiceLine>
        {
            Inv(1_000_000m, 110_000m)
        });

        var result = await _sut.GetPpnSummaryAsync(2026, 4);

        result.TotalPpnInput.Should().Be(0m);
        result.InputAvailable.Should().BeFalse();
        result.NetPpnPayable.Should().Be(110_000m);
    }

    [Fact]
    public async Task Groups_invoices_by_effective_tax_rate()
    {
        var rows = new List<PpnInvoiceLine>
        {
            Inv(1_000_000m, 110_000m),  // 11%
            Inv(500_000m, 55_000m),     // 11%
            Inv(2_000_000m, 240_000m),  // 12%
        };
        _reportRepo.GetPpnOutputInvoicesAsync(2026, 4).Returns(rows);

        var result = await _sut.GetPpnSummaryAsync(2026, 4);

        result.RateGroups.Should().HaveCount(2);
        var group11 = result.RateGroups.Single(g => g.RatePercent == 11m);
        var group12 = result.RateGroups.Single(g => g.RatePercent == 12m);
        group11.InvoiceCount.Should().Be(2);
        group11.Dpp.Should().Be(1_500_000m);
        group11.PpnAmount.Should().Be(165_000m);
        group12.InvoiceCount.Should().Be(1);
        group12.PpnAmount.Should().Be(240_000m);
    }

    [Fact]
    public async Task Groups_zero_dpp_invoices_under_zero_rate()
    {
        var rows = new List<PpnInvoiceLine>
        {
            Inv(0m, 0m),                  // exempt / fully discounted
            Inv(0m, 0m),
            Inv(1_000_000m, 110_000m)     // normal 11%
        };
        _reportRepo.GetPpnOutputInvoicesAsync(2026, 4).Returns(rows);

        var result = await _sut.GetPpnSummaryAsync(2026, 4);

        var zero = result.RateGroups.Single(g => g.RatePercent == 0m);
        zero.InvoiceCount.Should().Be(2);
        zero.TaxRateName.Should().Contain("Exempt");
    }

    [Fact]
    public async Task Sorts_rate_groups_highest_rate_first()
    {
        var rows = new List<PpnInvoiceLine>
        {
            Inv(0m, 0m),
            Inv(1_000_000m, 110_000m),
            Inv(2_000_000m, 240_000m)
        };
        _reportRepo.GetPpnOutputInvoicesAsync(2026, 4).Returns(rows);

        var result = await _sut.GetPpnSummaryAsync(2026, 4);

        result.RateGroups.Select(g => g.RatePercent).Should().Equal(12m, 11m, 0m);
    }

    [Fact]
    public async Task Returns_empty_report_when_no_invoices_in_month()
    {
        _reportRepo.GetPpnOutputInvoicesAsync(2026, 4).Returns(new List<PpnInvoiceLine>());

        var result = await _sut.GetPpnSummaryAsync(2026, 4);

        result.Year.Should().Be(2026);
        result.Month.Should().Be(4);
        result.TotalDpp.Should().Be(0m);
        result.TotalPpnOutput.Should().Be(0m);
        result.NetPpnPayable.Should().Be(0m);
        result.RateGroups.Should().BeEmpty();
        result.Invoices.Should().BeEmpty();
    }

    [Fact]
    public async Task Preserves_invoice_lines_for_csv_export()
    {
        var rows = new List<PpnInvoiceLine>
        {
            new()
            {
                InvoiceId = Guid.NewGuid(),
                InvoiceNumber = "INV-001",
                TaxInvoiceNumber = "010.000-26.12345678",
                CounterpartyName = "PT ABC",
                CounterpartyNPWP = "01.234.567.8-901.000",
                InvoiceDate = new DateTime(2026, 4, 10),
                Dpp = 1_000_000m,
                PpnAmount = 110_000m,
                GrandTotal = 1_110_000m,
                Status = "Issued"
            }
        };
        _reportRepo.GetPpnOutputInvoicesAsync(2026, 4).Returns(rows);

        var result = await _sut.GetPpnSummaryAsync(2026, 4);

        result.Invoices.Should().HaveCount(1);
        var inv = result.Invoices[0];
        inv.InvoiceNumber.Should().Be("INV-001");
        inv.TaxInvoiceNumber.Should().Be("010.000-26.12345678");
        inv.CounterpartyNPWP.Should().Be("01.234.567.8-901.000");
    }
}
