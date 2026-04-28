using Application.UseCases.Import;
using FluentAssertions;

namespace StockLedger.Unit.Tests.UseCases;

public class CsvHeaderAutoMapperTests
{
    // ── Regression: precedence bug from Index.razor:492 ──────────────────────
    // Original frontend logic was `(order && no) || pesanan` due to operator
    // precedence, so "Order ID" silently failed to map (no "no" substring,
    // no "pesanan"). Pinned here so it never regresses.

    [Fact]
    public void Maps_OrderId_header_from_real_marketplace_export()
    {
        var headers = new[] { "Order ID", "SKU", "Quantity", "Unit Price" };

        var mapping = CsvHeaderAutoMapper.Map(headers);

        mapping.OrderNumberColumn.Should().Be("Order ID");
    }

    [Fact]
    public void Maps_OrderNumber_header()
    {
        var headers = new[] { "Order Number", "SKU", "Qty", "Price" };
        var mapping = CsvHeaderAutoMapper.Map(headers);
        mapping.OrderNumberColumn.Should().Be("Order Number");
    }

    [Fact]
    public void Maps_NoPesanan_Bahasa_header()
    {
        var headers = new[] { "No. Pesanan", "SKU", "Jumlah", "Harga Satuan" };
        var mapping = CsvHeaderAutoMapper.Map(headers);
        mapping.OrderNumberColumn.Should().Be("No. Pesanan");
    }

    // ── Longest-keyword-wins: "Total Price" vs "Unit Price" ──────────────────
    // If both columns exist, "Total Price" must claim total-price field, NOT
    // unit-price field. Old logic checked "total" before "price" but greedily,
    // which could mismap if the order varied.

    [Fact]
    public void Maps_TotalPrice_and_UnitPrice_to_separate_fields_when_both_present()
    {
        var headers = new[] { "Order ID", "SKU", "Quantity", "Unit Price", "Total Price" };

        var mapping = CsvHeaderAutoMapper.Map(headers);

        mapping.UnitPriceColumn.Should().Be("Unit Price");
        mapping.TotalPriceColumn.Should().Be("Total Price");
    }

    [Fact]
    public void Maps_TotalHarga_and_HargaSatuan_to_separate_fields_Bahasa()
    {
        var headers = new[] { "No. Pesanan", "SKU", "Jumlah", "Harga Satuan", "Total Harga" };

        var mapping = CsvHeaderAutoMapper.Map(headers);

        mapping.UnitPriceColumn.Should().Be("Harga Satuan");
        mapping.TotalPriceColumn.Should().Be("Total Harga");
    }

    // ── Subtotal should claim total-price, not get lost ──────────────────────
    [Fact]
    public void Maps_Subtotal_header_to_total_price_field()
    {
        var headers = new[] { "Order ID", "SKU", "Qty", "Unit Price", "Subtotal" };
        var mapping = CsvHeaderAutoMapper.Map(headers);
        mapping.TotalPriceColumn.Should().Be("Subtotal");
    }

    // ── No claim collision: each header maps to at most one field ────────────
    [Fact]
    public void Each_header_is_claimed_by_at_most_one_field()
    {
        var headers = new[] { "Order ID", "SKU", "Qty", "Price", "Total", "Product Name", "Date", "Fee" };

        var mapping = CsvHeaderAutoMapper.Map(headers);

        var assignments = new[]
        {
            mapping.OrderNumberColumn,
            mapping.SkuColumn,
            mapping.QuantityColumn,
            mapping.UnitPriceColumn,
            mapping.TotalPriceColumn,
            mapping.ProductNameColumn,
            mapping.OrderDateColumn,
            mapping.PlatformFeeColumn,
        }.Where(s => !string.IsNullOrEmpty(s)).ToArray();

        assignments.Should().OnlyHaveUniqueItems();
    }

    // ── Required fields populated from minimal header set ────────────────────
    [Fact]
    public void Maps_minimal_required_fields_from_synthetic_Tokopedia_fixture()
    {
        var headers = new[] { "no_pesanan", "sku", "nama_produk", "jumlah", "harga_satuan", "total_harga", "tanggal" };

        var mapping = CsvHeaderAutoMapper.Map(headers);

        mapping.OrderNumberColumn.Should().Be("no_pesanan");
        mapping.SkuColumn.Should().Be("sku");
        mapping.QuantityColumn.Should().Be("jumlah");
        mapping.UnitPriceColumn.Should().Be("harga_satuan");
        mapping.TotalPriceColumn.Should().Be("total_harga");
        mapping.ProductNameColumn.Should().Be("nama_produk");
        mapping.OrderDateColumn.Should().Be("tanggal");
    }

    [Fact]
    public void Maps_minimal_required_fields_from_synthetic_Shopee_fixture()
    {
        var headers = new[] { "order_number", "sku", "product_name", "quantity", "unit_price", "total_price", "order_date", "platform_fee" };

        var mapping = CsvHeaderAutoMapper.Map(headers);

        mapping.OrderNumberColumn.Should().Be("order_number");
        mapping.SkuColumn.Should().Be("sku");
        mapping.QuantityColumn.Should().Be("quantity");
        mapping.UnitPriceColumn.Should().Be("unit_price");
        mapping.TotalPriceColumn.Should().Be("total_price");
        mapping.ProductNameColumn.Should().Be("product_name");
        mapping.OrderDateColumn.Should().Be("order_date");
        mapping.PlatformFeeColumn.Should().Be("platform_fee");
    }

    // ── Real-export header pinned cases (likely Tokopedia/Shopee Seller Center exports) ──
    // These are educated guesses based on public marketplace documentation; will be
    // replaced or augmented with actual headers once a real export is provided.

    [Fact]
    public void Maps_real_Shopee_style_export_headers()
    {
        // Shopee Seller Center exports typically include English column names.
        var headers = new[]
        {
            "Order ID", "Order Status", "Tracking Number", "SKU", "Product Name",
            "Quantity", "Unit Price", "Total Amount", "Order Creation Date", "Platform Fee"
        };

        var mapping = CsvHeaderAutoMapper.Map(headers);

        mapping.OrderNumberColumn.Should().Be("Order ID");
        mapping.SkuColumn.Should().Be("SKU");
        mapping.QuantityColumn.Should().Be("Quantity");
        mapping.UnitPriceColumn.Should().Be("Unit Price");
        mapping.TotalPriceColumn.Should().Be("Total Amount");
        mapping.ProductNameColumn.Should().Be("Product Name");
        mapping.OrderDateColumn.Should().Be("Order Creation Date");
        mapping.PlatformFeeColumn.Should().Be("Platform Fee");
    }

    [Fact]
    public void Maps_real_Tokopedia_style_export_headers()
    {
        // Tokopedia Seller Center exports typically use Bahasa headers.
        var headers = new[]
        {
            "No. Pesanan", "Tanggal Pembayaran", "Status Pesanan", "SKU", "Nama Produk",
            "Jumlah", "Harga Satuan", "Total Harga", "Biaya Admin"
        };

        var mapping = CsvHeaderAutoMapper.Map(headers);

        mapping.OrderNumberColumn.Should().Be("No. Pesanan");
        mapping.SkuColumn.Should().Be("SKU");
        mapping.QuantityColumn.Should().Be("Jumlah");
        mapping.UnitPriceColumn.Should().Be("Harga Satuan");
        mapping.TotalPriceColumn.Should().Be("Total Harga");
        mapping.ProductNameColumn.Should().Be("Nama Produk");
        mapping.OrderDateColumn.Should().Be("Tanggal Pembayaran");
        mapping.PlatformFeeColumn.Should().Be("Biaya Admin");
    }

    // ── Edge cases ───────────────────────────────────────────────────────────

    [Fact]
    public void Returns_empty_required_fields_when_headers_missing()
    {
        var headers = new[] { "Random Column", "Another Column" };

        var mapping = CsvHeaderAutoMapper.Map(headers);

        mapping.OrderNumberColumn.Should().BeEmpty();
        mapping.SkuColumn.Should().BeEmpty();
        mapping.QuantityColumn.Should().BeEmpty();
        mapping.UnitPriceColumn.Should().BeEmpty();
    }

    [Fact]
    public void Handles_empty_header_list()
    {
        var mapping = CsvHeaderAutoMapper.Map(Array.Empty<string>());

        mapping.OrderNumberColumn.Should().BeEmpty();
        mapping.PlatformFeeColumn.Should().BeNull();
    }

    [Fact]
    public void Skips_blank_or_whitespace_headers()
    {
        var headers = new[] { "", "  ", "Order ID", "SKU", "Qty", "Price" };

        var mapping = CsvHeaderAutoMapper.Map(headers);

        mapping.OrderNumberColumn.Should().Be("Order ID");
    }

    [Fact]
    public void Throws_on_null_headers()
    {
        Action act = () => CsvHeaderAutoMapper.Map(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Is_case_insensitive_for_header_matching()
    {
        var headers = new[] { "ORDER ID", "sku", "QUANTITY", "Unit PRICE" };

        var mapping = CsvHeaderAutoMapper.Map(headers);

        mapping.OrderNumberColumn.Should().Be("ORDER ID");
        mapping.SkuColumn.Should().Be("sku");
        mapping.QuantityColumn.Should().Be("QUANTITY");
        mapping.UnitPriceColumn.Should().Be("Unit PRICE");
    }
}
