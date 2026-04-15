using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using StockLedger.E2E.Tests.Helpers;

namespace StockLedger.E2E.Tests.Flows;

public class Flow03_CsvImportTests : StockLedgerTestBase
{
    [Fact]
    public async Task CsvImport_FullJourney()
    {
        var token = await LoginAsync("admin@stockledger.test", "Admin123!@#");

        // ── Step 1: Create sales channel ─────────────────────────────────
        var channelResp = await AuthPost(token, $"{V1}/import/channels", new
        {
            name = "Tokopedia",
            description = "Tokopedia marketplace",
            platformFeePercent = 5.0m
        });
        channelResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var channel = await ReadAs<JsonElement>(channelResp);
        var channelId = channel.GetProperty("id").GetString();
        channelId.Should().NotBeNullOrEmpty();

        // ── Step 2: Ensure we have a warehouse and product variants ──────
        // (These should exist from Flow01 seed data)
        var warehousesResp = await AuthGet(token, $"{V1}/warehouses?page=1&pageSize=1");
        warehousesResp.EnsureSuccessStatusCode();
        var warehouses = await ReadAs<PagedResult<JsonElement>>(warehousesResp);
        warehouses.Items.Should().NotBeEmpty("at least one warehouse should exist from seed data");
        var warehouseId = warehouses.Items[0].GetProperty("id").GetString();

        // ── Step 3: Preview CSV headers ──────────────────────────────────
        var csvContent = "Nomor Pesanan,Nomor SKU,Jumlah Produk,Harga (IDR)\nORD-001,NON-EXISTENT-SKU,2,50000\nORD-002,NON-EXISTENT-SKU-2,1,75000";
        var previewResp = await UploadCsv(token, $"{V1}/import/csv/preview", csvContent, "tokopedia-test.csv");
        previewResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var headers = await ReadAs<List<string>>(previewResp);
        headers.Should().Contain("Nomor Pesanan");
        headers.Should().Contain("Nomor SKU");

        // ── Step 4: Process import (SKUs won't match — tests error handling) ─
        var formContent = new MultipartFormDataContent();
        formContent.Add(new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(csvContent))), "file", "tokopedia-test.csv");
        formContent.Add(new StringContent(channelId!), "salesChannelId");
        formContent.Add(new StringContent(warehouseId!), "warehouseId");
        formContent.Add(new StringContent("Nomor Pesanan"), "orderNumberColumn");
        formContent.Add(new StringContent("Nomor SKU"), "skuColumn");
        formContent.Add(new StringContent("Jumlah Produk"), "quantityColumn");
        formContent.Add(new StringContent("Harga (IDR)"), "unitPriceColumn");

        var processReq = new HttpRequestMessage(HttpMethod.Post, $"{V1}/import/csv/process");
        processReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        processReq.Headers.Add("X-Tenant-Id", DefaultTenantId);
        processReq.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
        processReq.Content = formContent;
        var processResp = await Client.SendAsync(processReq);
        processResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var summary = await ReadAs<JsonElement>(processResp);
        summary.GetProperty("totalRows").GetInt32().Should().Be(2);
        // Both rows should fail since SKUs don't exist
        summary.GetProperty("failedRows").GetInt32().Should().Be(2);

        // ── Step 5: Verify import batch history ──────────────────────────
        var batchesResp = await AuthGet(token, $"{V1}/import/batches?page=1&pageSize=10");
        batchesResp.EnsureSuccessStatusCode();
        var batches = await ReadAs<PagedResult<JsonElement>>(batchesResp);
        batches.Items.Should().NotBeEmpty();
        batches.Items[0].GetProperty("fileName").GetString().Should().Be("tokopedia-test.csv");
    }

    private static async Task<HttpResponseMessage> UploadCsv(string token, string url, string csvContent, string fileName)
    {
        var formContent = new MultipartFormDataContent();
        formContent.Add(new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(csvContent))), "file", fileName);

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Tenant-Id", DefaultTenantId);
        request.Content = formContent;
        return await Client.SendAsync(request);
    }
}
