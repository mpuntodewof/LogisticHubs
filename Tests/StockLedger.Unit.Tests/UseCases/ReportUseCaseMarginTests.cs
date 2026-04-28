using Application.DTOs.Reports;
using Application.Interfaces;
using Application.UseCases.Reports;
using FluentAssertions;
using NSubstitute;

namespace StockLedger.Unit.Tests.UseCases;

// Margin math lives in ReportUseCase, not the repository. The repo returns
// raw aggregates (Revenue, TotalCost, PlatformFees); the use case computes
// NetMargin + NetMarginPercent + worst-first sort + grand totals.
// These tests pin that math.
public class ReportUseCaseMarginTests
{
    private readonly IReportRepository _reportRepo = Substitute.For<IReportRepository>();
    private readonly IInvoiceRepository _invoiceRepo = Substitute.For<IInvoiceRepository>();
    private readonly ReportUseCase _sut;

    private static readonly DateTime _from = new(2026, 4, 1);
    private static readonly DateTime _to = new(2026, 4, 30);

    public ReportUseCaseMarginTests()
    {
        _sut = new ReportUseCase(_reportRepo, _invoiceRepo);
    }

    [Fact]
    public async Task Computes_net_margin_and_percent_per_product()
    {
        var raw = new List<ProductMarginLine>
        {
            new()
            {
                Sku = "SKU-A", ProductName = "Product A",
                UnitsSold = 10, Revenue = 1_000_000m,
                CostPrice = 60_000m, TotalCost = 600_000m,
                PlatformFees = 100_000m
            }
        };
        _reportRepo.GetProductMarginsAsync(_from, _to).Returns(raw);
        _reportRepo.GetProductChannelMarginsAsync(_from, _to).Returns(new List<ProductChannelMarginLine>());

        var result = await _sut.GetProductMarginReportAsync(_from, _to);

        var p = result.Products.Single();
        p.NetMargin.Should().Be(300_000m);             // 1_000_000 - 600_000 - 100_000
        p.NetMarginPercent.Should().Be(30m);           // 300_000 / 1_000_000 * 100
    }

    [Fact]
    public async Task Sorts_products_worst_margin_first()
    {
        var raw = new List<ProductMarginLine>
        {
            new() { Sku = "GOOD", Revenue = 1000m, TotalCost = 500m, PlatformFees = 100m },     // 40% margin
            new() { Sku = "BAD",  Revenue = 1000m, TotalCost = 900m, PlatformFees = 100m },     // 0% margin
            new() { Sku = "OK",   Revenue = 1000m, TotalCost = 700m, PlatformFees = 100m }      // 20% margin
        };
        _reportRepo.GetProductMarginsAsync(_from, _to).Returns(raw);
        _reportRepo.GetProductChannelMarginsAsync(_from, _to).Returns(new List<ProductChannelMarginLine>());

        var result = await _sut.GetProductMarginReportAsync(_from, _to);

        result.Products.Select(p => p.Sku).Should().Equal("BAD", "OK", "GOOD");
    }

    [Fact]
    public async Task Surfaces_loss_making_products_with_negative_margin()
    {
        var raw = new List<ProductMarginLine>
        {
            new() { Sku = "LOSS", Revenue = 100_000m, TotalCost = 80_000m, PlatformFees = 30_000m }
            // Net margin = -10_000, margin% = -10
        };
        _reportRepo.GetProductMarginsAsync(_from, _to).Returns(raw);
        _reportRepo.GetProductChannelMarginsAsync(_from, _to).Returns(new List<ProductChannelMarginLine>());

        var result = await _sut.GetProductMarginReportAsync(_from, _to);

        var p = result.Products.Single();
        p.NetMargin.Should().Be(-10_000m);
        p.NetMarginPercent.Should().Be(-10m);
    }

    [Fact]
    public async Task Returns_zero_margin_percent_when_revenue_is_zero()
    {
        var raw = new List<ProductMarginLine>
        {
            new() { Sku = "FREE", Revenue = 0m, TotalCost = 0m, PlatformFees = 0m }
        };
        _reportRepo.GetProductMarginsAsync(_from, _to).Returns(raw);
        _reportRepo.GetProductChannelMarginsAsync(_from, _to).Returns(new List<ProductChannelMarginLine>());

        var result = await _sut.GetProductMarginReportAsync(_from, _to);

        result.Products.Single().NetMarginPercent.Should().Be(0m);
        result.NetMarginPercent.Should().Be(0m);
    }

    [Fact]
    public async Task Computes_grand_totals_across_all_products()
    {
        var raw = new List<ProductMarginLine>
        {
            new() { Sku = "A", Revenue = 1000m, TotalCost = 500m, PlatformFees = 100m },
            new() { Sku = "B", Revenue = 2000m, TotalCost = 1200m, PlatformFees = 200m }
        };
        _reportRepo.GetProductMarginsAsync(_from, _to).Returns(raw);
        _reportRepo.GetProductChannelMarginsAsync(_from, _to).Returns(new List<ProductChannelMarginLine>());

        var result = await _sut.GetProductMarginReportAsync(_from, _to);

        result.TotalRevenue.Should().Be(3000m);
        result.TotalCogs.Should().Be(1700m);
        result.TotalPlatformFees.Should().Be(300m);
        result.TotalNetMargin.Should().Be(1000m);                          // 3000 - 1700 - 300
        result.NetMarginPercent.Should().BeApproximately(33.33m, 0.01m);   // 1000 / 3000 * 100
    }

    [Fact]
    public async Task Computes_per_product_per_channel_margin_independently()
    {
        var byChannel = new List<ProductChannelMarginLine>
        {
            new() { Sku = "A", ChannelName = "Tokopedia", Revenue = 1000m, TotalCost = 500m, PlatformFees = 100m },
            new() { Sku = "A", ChannelName = "Shopee",    Revenue = 1000m, TotalCost = 500m, PlatformFees = 200m }
        };
        _reportRepo.GetProductMarginsAsync(_from, _to).Returns(new List<ProductMarginLine>());
        _reportRepo.GetProductChannelMarginsAsync(_from, _to).Returns(byChannel);

        var result = await _sut.GetProductMarginReportAsync(_from, _to);

        var toko = result.ProductByChannel.Single(c => c.ChannelName == "Tokopedia");
        var shopee = result.ProductByChannel.Single(c => c.ChannelName == "Shopee");
        toko.NetMargin.Should().Be(400m);            // higher fee on Shopee
        shopee.NetMargin.Should().Be(300m);
        toko.NetMarginPercent.Should().Be(40m);
        shopee.NetMarginPercent.Should().Be(30m);
    }

    [Fact]
    public async Task Sorts_per_channel_rows_worst_first()
    {
        var byChannel = new List<ProductChannelMarginLine>
        {
            new() { Sku = "A", ChannelName = "GOOD", Revenue = 1000m, TotalCost = 500m, PlatformFees = 100m }, // 40%
            new() { Sku = "A", ChannelName = "BAD",  Revenue = 1000m, TotalCost = 900m, PlatformFees = 100m }, // 0%
        };
        _reportRepo.GetProductMarginsAsync(_from, _to).Returns(new List<ProductMarginLine>());
        _reportRepo.GetProductChannelMarginsAsync(_from, _to).Returns(byChannel);

        var result = await _sut.GetProductMarginReportAsync(_from, _to);

        result.ProductByChannel.Select(c => c.ChannelName).Should().Equal("BAD", "GOOD");
    }

    [Fact]
    public async Task Returns_empty_report_when_no_data()
    {
        _reportRepo.GetProductMarginsAsync(_from, _to).Returns(new List<ProductMarginLine>());
        _reportRepo.GetProductChannelMarginsAsync(_from, _to).Returns(new List<ProductChannelMarginLine>());

        var result = await _sut.GetProductMarginReportAsync(_from, _to);

        result.Products.Should().BeEmpty();
        result.ProductByChannel.Should().BeEmpty();
        result.TotalRevenue.Should().Be(0m);
        result.NetMarginPercent.Should().Be(0m);
        result.FromDate.Should().Be(_from);
        result.ToDate.Should().Be(_to);
    }
}
