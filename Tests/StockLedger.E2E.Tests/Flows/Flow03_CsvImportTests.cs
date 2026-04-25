using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using StockLedger.E2E.Tests.Helpers;

namespace StockLedger.E2E.Tests.Flows;

public class Flow03_CsvImportTests : StockLedgerTestBase
{
    // ── Seed SKUs that exist in the default tenant (see AppDbContext HasData) ──
    private const string SkuIndomie = "IDM-GRG-001";      // Indomie Mi Goreng - Single Pack
    private const string SkuAqua600 = "AQU-600-001";      // Aqua 600ml
    private const string SkuKapalApi = "KPA-SPL-165";     // Kapal Api Special 165g

    private static Task<string> GetTokenAsync() => LoginAsync("admin@stockledger.io", "password123");

    // ── Test 1: InitialStock import succeeds when variants exist ────────────

    [Fact]
    public async Task InitialStock_ImportsSuccessfully_WhenVariantsExist()
    {
        var token = await GetTokenAsync();
        var warehouseId = await CreateWarehouseAsync(token);

        var csv = "sku,quantity\n" +
                  $"{SkuIndomie},200\n" +
                  $"{SkuAqua600},300\n" +
                  $"{SkuKapalApi},150\n";

        var resp = await PostInitialStockAsync(token, warehouseId, csv, "initial-stock.csv");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await ReadAs<JsonElement>(resp);
        result.GetProperty("totalRows").GetInt32().Should().Be(3);
        result.GetProperty("successRows").GetInt32().Should().Be(3);
        result.GetProperty("failedRows").GetInt32().Should().Be(0);

        var stock = await GetStockAsync(token, warehouseId, SkuIndomie);
        stock.Should().Be(200);
    }

    // ── Test 2: Sales import deducts stock and creates movements ────────────

    [Fact]
    public async Task SalesImport_DeductsStock_AndRecordsMovements()
    {
        var token = await GetTokenAsync();
        var warehouseId = await CreateWarehouseAsync(token);
        var channelId = await CreateChannelAsync(token, "Tokopedia", 12.0m);

        // Seed: Indomie = 100
        await PostInitialStockAsync(token, warehouseId,
            "sku,quantity\n" + $"{SkuIndomie},100\n",
            "seed.csv");

        var qtyBefore = await GetStockAsync(token, warehouseId, SkuIndomie);
        qtyBefore.Should().Be(100);

        // Sale: 15 units
        var csv = "no_pesanan,sku,jumlah,harga_satuan\n" +
                  $"ORD-001,{SkuIndomie},15,3000\n";
        var resp = await PostSalesImportAsync(token, channelId, warehouseId, csv, "sales.csv",
            orderNumberColumn: "no_pesanan", skuColumn: "sku",
            quantityColumn: "jumlah", unitPriceColumn: "harga_satuan");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var summary = await ReadAs<JsonElement>(resp);
        summary.GetProperty("successRows").GetInt32().Should().Be(1);

        var qtyAfter = await GetStockAsync(token, warehouseId, SkuIndomie);
        qtyAfter.Should().Be(85, "sale of 15 units should deduct from on-hand");

        // Verify an "Out / Sale" movement was created referencing this batch
        var movementsResp = await AuthGet(token, $"{V1}/stock-movements?warehouseId={warehouseId}&movementType=Out&page=1&pageSize=10");
        movementsResp.EnsureSuccessStatusCode();
        var movements = await ReadAs<PagedResult<JsonElement>>(movementsResp);
        movements.Items.Should().Contain(m =>
            m.GetProperty("sku").GetString() == SkuIndomie &&
            m.GetProperty("reason").GetString() == "Sale" &&
            m.GetProperty("quantity").GetInt32() == 15);
    }

    // ── Test 3: Duplicate order numbers are skipped on re-import ────────────

    [Fact]
    public async Task SalesImport_IsIdempotent_OnDuplicateOrderNumbers()
    {
        var token = await GetTokenAsync();
        var warehouseId = await CreateWarehouseAsync(token);
        var channelId = await CreateChannelAsync(token, "Shopee", 15.0m);

        await PostInitialStockAsync(token, warehouseId,
            "sku,quantity\n" + $"{SkuAqua600},50\n",
            "seed.csv");

        var csv = "no_pesanan,sku,jumlah,harga_satuan\n" +
                  $"ORD-DUP-001,{SkuAqua600},10,4500\n";

        // First import: 10 units sold, stock = 40
        var first = await PostSalesImportAsync(token, channelId, warehouseId, csv, "sales1.csv",
            orderNumberColumn: "no_pesanan", skuColumn: "sku",
            quantityColumn: "jumlah", unitPriceColumn: "harga_satuan");
        first.StatusCode.Should().Be(HttpStatusCode.OK);
        var firstSummary = await ReadAs<JsonElement>(first);
        firstSummary.GetProperty("successRows").GetInt32().Should().Be(1);

        var qtyAfterFirst = await GetStockAsync(token, warehouseId, SkuAqua600);
        qtyAfterFirst.Should().Be(40);

        // Second import of same CSV: must skip as duplicate, stock unchanged
        var second = await PostSalesImportAsync(token, channelId, warehouseId, csv, "sales2.csv",
            orderNumberColumn: "no_pesanan", skuColumn: "sku",
            quantityColumn: "jumlah", unitPriceColumn: "harga_satuan");
        second.StatusCode.Should().Be(HttpStatusCode.OK);
        var secondSummary = await ReadAs<JsonElement>(second);
        secondSummary.GetProperty("successRows").GetInt32().Should().Be(0);
        secondSummary.GetProperty("duplicateRows").GetInt32().Should().Be(1);

        var qtyAfterSecond = await GetStockAsync(token, warehouseId, SkuAqua600);
        qtyAfterSecond.Should().Be(40, "duplicate import must not double-deduct");
    }

    // ── Test 4: Import rejects row when quantity exceeds on-hand ────────────

    [Fact]
    public async Task SalesImport_Rejects_NegativeStockAttempt()
    {
        var token = await GetTokenAsync();
        var warehouseId = await CreateWarehouseAsync(token);
        var channelId = await CreateChannelAsync(token, "Tokopedia", 12.0m);

        // Seed: only 10 on hand
        await PostInitialStockAsync(token, warehouseId,
            "sku,quantity\n" + $"{SkuKapalApi},10\n",
            "seed.csv");

        // Try to sell 20
        var csv = "no_pesanan,sku,jumlah,harga_satuan\n" +
                  $"ORD-OVERSELL-001,{SkuKapalApi},20,17500\n";
        var resp = await PostSalesImportAsync(token, channelId, warehouseId, csv, "sales.csv",
            orderNumberColumn: "no_pesanan", skuColumn: "sku",
            quantityColumn: "jumlah", unitPriceColumn: "harga_satuan");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var summary = await ReadAs<JsonElement>(resp);
        summary.GetProperty("successRows").GetInt32().Should().Be(0);
        summary.GetProperty("failedRows").GetInt32().Should().Be(1);

        // Stock remains 10 — no negative balance
        var qty = await GetStockAsync(token, warehouseId, SkuKapalApi);
        qty.Should().Be(10, "overselling must not drive on-hand negative");

        var failedDetails = summary.GetProperty("failedRowDetails");
        failedDetails.GetArrayLength().Should().Be(1);
        failedDetails[0].GetProperty("errorMessage").GetString()
            .Should().Contain("Insufficient stock");
    }

    // ── Test 5: Malformed CSV handled gracefully ─────────────────────────────

    [Fact]
    public async Task SalesImport_HandlesMalformedCsv_Gracefully()
    {
        var token = await GetTokenAsync();
        var warehouseId = await CreateWarehouseAsync(token);
        var channelId = await CreateChannelAsync(token, "Tokopedia", 12.0m);

        // Empty file — API should return a 4xx, not 500
        var empty = await PostSalesImportAsync(token, channelId, warehouseId, "", "empty.csv",
            orderNumberColumn: "no_pesanan", skuColumn: "sku",
            quantityColumn: "jumlah", unitPriceColumn: "harga_satuan");
        empty.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Conflict);

        // Headers only, no data rows — must reject, not partial-commit
        var headersOnly = "no_pesanan,sku,jumlah,harga_satuan\n";
        var resp = await PostSalesImportAsync(token, channelId, warehouseId, headersOnly, "headers.csv",
            orderNumberColumn: "no_pesanan", skuColumn: "sku",
            quantityColumn: "jumlah", unitPriceColumn: "harga_satuan");
        resp.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Conflict);
    }

    // ── Test 6: Shopee platform fees captured from CSV column ────────────────

    [Fact]
    public async Task SalesImport_CapturesPlatformFees_ForShopee()
    {
        var token = await GetTokenAsync();
        var warehouseId = await CreateWarehouseAsync(token);
        var channelId = await CreateChannelAsync(token, "Shopee", 15.0m);

        await PostInitialStockAsync(token, warehouseId,
            "sku,quantity\n" + $"{SkuAqua600},100\n",
            "seed.csv");

        // 10 units @ 4500 = 45,000 total; CSV fee = 6,750 (exactly 15%)
        var csv = "order_number,sku,quantity,unit_price,total_price,platform_fee\n" +
                  $"SHP-FEE-001,{SkuAqua600},10,4500,45000,6750\n";
        var resp = await PostSalesImportAsync(token, channelId, warehouseId, csv, "shopee.csv",
            orderNumberColumn: "order_number", skuColumn: "sku",
            quantityColumn: "quantity", unitPriceColumn: "unit_price",
            totalPriceColumn: "total_price", platformFeeColumn: "platform_fee");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var summary = await ReadAs<JsonElement>(resp);
        var batchId = summary.GetProperty("batchId").GetString();

        var detail = await AuthGet(token, $"{V1}/import/batches/{batchId}");
        detail.EnsureSuccessStatusCode();
        var batch = await ReadAs<JsonElement>(detail);
        var row = batch.GetProperty("rows")[0];
        row.GetProperty("platformFee").GetDecimal().Should().Be(6750m,
            "Shopee CSV provides platform_fee explicitly — importer must use it, not the channel default");
        row.GetProperty("totalPrice").GetDecimal().Should().Be(45000m);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static async Task<string> CreateWarehouseAsync(string token)
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var resp = await AuthPost(token, $"{V1}/warehouses", new
        {
            name = $"E2E-Flow03-{suffix}",
            location = "E2E test warehouse",
            capacity = 10000
        });
        resp.EnsureSuccessStatusCode();
        var w = await ReadAs<JsonElement>(resp);
        return w.GetProperty("id").GetString()!;
    }

    private static async Task<string> CreateChannelAsync(string token, string baseName, decimal feePercent)
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var resp = await AuthPost(token, $"{V1}/import/channels", new
        {
            name = $"{baseName}-{suffix}",
            description = $"E2E {baseName}",
            platformFeePercent = feePercent
        });
        resp.EnsureSuccessStatusCode();
        var c = await ReadAs<JsonElement>(resp);
        return c.GetProperty("id").GetString()!;
    }

    private static async Task<HttpResponseMessage> PostInitialStockAsync(
        string token, string warehouseId, string csv, string fileName)
    {
        var form = new MultipartFormDataContent
        {
            { new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(csv))), "file", fileName },
            { new StringContent(warehouseId), "warehouseId" },
            { new StringContent("sku"), "skuColumn" },
            { new StringContent("quantity"), "quantityColumn" },
        };
        return await SendMultipartAsync(token, $"{V1}/import/csv/initial-stock", form);
    }

    private static async Task<HttpResponseMessage> PostSalesImportAsync(
        string token, string channelId, string warehouseId, string csv, string fileName,
        string orderNumberColumn, string skuColumn, string quantityColumn, string unitPriceColumn,
        string? totalPriceColumn = null, string? platformFeeColumn = null)
    {
        var form = new MultipartFormDataContent
        {
            { new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(csv))), "file", fileName },
            { new StringContent(channelId), "salesChannelId" },
            { new StringContent(warehouseId), "warehouseId" },
            { new StringContent(orderNumberColumn), "orderNumberColumn" },
            { new StringContent(skuColumn), "skuColumn" },
            { new StringContent(quantityColumn), "quantityColumn" },
            { new StringContent(unitPriceColumn), "unitPriceColumn" },
        };
        if (totalPriceColumn != null) form.Add(new StringContent(totalPriceColumn), "totalPriceColumn");
        if (platformFeeColumn != null) form.Add(new StringContent(platformFeeColumn), "platformFeeColumn");
        return await SendMultipartAsync(token, $"{V1}/import/csv/process", form);
    }

    private static async Task<HttpResponseMessage> SendMultipartAsync(string token, string url, MultipartFormDataContent form)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        req.Headers.Add("X-Tenant-Id", DefaultTenantId);
        req.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
        req.Content = form;
        return await Client.SendAsync(req);
    }

    private static async Task<int> GetStockAsync(string token, string warehouseId, string sku)
    {
        var resp = await AuthGet(token, $"{V1}/warehouse-stock?page=1&pageSize=100");
        resp.EnsureSuccessStatusCode();
        var paged = await ReadAs<PagedResult<JsonElement>>(resp);
        foreach (var item in paged.Items)
        {
            if (item.GetProperty("warehouseId").GetString() == warehouseId &&
                item.GetProperty("sku").GetString() == sku)
            {
                return item.GetProperty("quantityOnHand").GetInt32();
            }
        }
        return 0;
    }
}
