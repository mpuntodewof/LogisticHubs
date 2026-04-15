using FluentAssertions;
using Infrastructure.Services;

namespace StockLedger.Unit.Tests.UseCases;

public class CsvParserServiceTests
{
    private readonly CsvParserService _parser = new();

    private static Stream ToStream(string content)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    [Fact]
    public async Task ParseAsync_SimpleCsv_ReturnsRows()
    {
        var csv = "OrderNo,SKU,Qty,Price\n001,SKU-A,5,10000\n002,SKU-B,3,25000";
        using var stream = ToStream(csv);

        var result = await _parser.ParseAsync(stream);

        result.Should().HaveCount(2);
        result[0]["OrderNo"].Should().Be("001");
        result[0]["SKU"].Should().Be("SKU-A");
        result[0]["Qty"].Should().Be("5");
        result[0]["Price"].Should().Be("10000");
        result[1]["OrderNo"].Should().Be("002");
    }

    [Fact]
    public async Task ParseAsync_QuotedFields_HandlesCorrectly()
    {
        var csv = "Name,Description,Price\n\"Product A\",\"Has a, comma\",15000";
        using var stream = ToStream(csv);

        var result = await _parser.ParseAsync(stream);

        result.Should().HaveCount(1);
        result[0]["Name"].Should().Be("Product A");
        result[0]["Description"].Should().Be("Has a, comma");
    }

    [Fact]
    public async Task ParseAsync_EmptyCsv_ReturnsEmpty()
    {
        using var stream = ToStream("");

        var result = await _parser.ParseAsync(stream);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ParseAsync_HeaderOnly_ReturnsEmpty()
    {
        using var stream = ToStream("Col1,Col2,Col3\n");

        var result = await _parser.ParseAsync(stream);

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetHeaders_ReturnsTrimmedHeaders()
    {
        using var stream = ToStream("  Order No , SKU , Quantity ,Price\ndata");

        var headers = _parser.GetHeaders(stream);

        headers.Should().BeEquivalentTo("Order No", "SKU", "Quantity", "Price");
    }

    [Fact]
    public async Task ParseAsync_CaseInsensitiveLookup()
    {
        var csv = "sku,QTY\nABC-123,10";
        using var stream = ToStream(csv);

        var result = await _parser.ParseAsync(stream);

        result[0]["SKU"].Should().Be("ABC-123");
        result[0]["qty"].Should().Be("10");
    }

    [Fact]
    public async Task ParseAsync_TokopediaFormat_Works()
    {
        var csv = "Nomor Pesanan,Nomor SKU,Jumlah Produk Dibeli,Harga Awal (IDR)\nINV/20260414/001,SKU-TOK-001,2,\"150,000\"";
        using var stream = ToStream(csv);

        var result = await _parser.ParseAsync(stream);

        result.Should().HaveCount(1);
        result[0]["Nomor Pesanan"].Should().Be("INV/20260414/001");
        result[0]["Nomor SKU"].Should().Be("SKU-TOK-001");
        result[0]["Jumlah Produk Dibeli"].Should().Be("2");
        result[0]["Harga Awal (IDR)"].Should().Be("150,000");
    }
}
