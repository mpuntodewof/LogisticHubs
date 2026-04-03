using System.Net;
using System.Text.Json;
using FluentAssertions;
using NiagaOne.E2E.Tests.Helpers;

namespace NiagaOne.E2E.Tests.Flows;

public class Flow02_PurchaseAndStockInTests : NiagaOneTestBase
{
    [Fact]
    public async Task Manager_Should_Create_PO_And_Warehouse_Should_Receive_Stock()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];

        // --- Login as manager ---
        var managerToken = await LoginAsync("manager@niagaone.com", "Manager@123");
        managerToken.Should().NotBeNullOrWhiteSpace("manager login should return a valid token");

        // --- Create Supplier ---
        var supplierRes = await AuthPost(managerToken, "/api/suppliers", new
        {
            name = $"PT Samsung Indonesia-{suffix}",
            code = $"SMSI-{suffix}"[..10],
            email = $"samsung-{suffix}@supplier.id",
            phone = "021-5551234",
            address = "Jl. Jenderal Sudirman No.45, Jakarta Selatan",
            status = "Active"
        });
        supplierRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating supplier should succeed");
        var supplier = await ReadAs<JsonElement>(supplierRes);
        var supplierId = supplier.GetProperty("id").GetString()!;
        TestDataStore.Set("supplierId", supplierId);

        // --- Get variant IDs from TestDataStore ---
        var galaxy128Id = TestDataStore.Get<string>("galaxy128VariantId");
        var galaxy256Id = TestDataStore.Get<string>("galaxy256VariantId");
        var warehouseId = TestDataStore.Get<string>("warehouseId");

        // --- Create Purchase Order ---
        var poRes = await AuthPost(managerToken, "/api/purchase-orders", new
        {
            supplierId,
            warehouseId,
            notes = "Initial stock purchase for Galaxy S24 variants",
            items = new[]
            {
                new { productVariantId = galaxy128Id, quantity = 50, unitPrice = 8000000m },
                new { productVariantId = galaxy256Id, quantity = 30, unitPrice = 9000000m }
            }
        });
        poRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating purchase order should succeed");
        var po = await ReadAs<JsonElement>(poRes);
        var poId = po.GetProperty("id").GetString()!;
        TestDataStore.Set("poId", poId);

        // --- Submit PO ---
        var submitRes = await AuthPost(managerToken, $"/api/purchase-orders/{poId}/submit");
        submitRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.OK, HttpStatusCode.NoContent },
            "submitting PO should succeed");

        // --- Approve PO ---
        var approveRes = await AuthPost(managerToken, $"/api/purchase-orders/{poId}/approve");
        approveRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.OK, HttpStatusCode.NoContent },
            "approving PO should succeed");

        // --- Verify PO Status ---
        var poDetailRes = await AuthGet(managerToken, $"/api/purchase-orders/{poId}");
        poDetailRes.StatusCode.Should().Be(HttpStatusCode.OK, "fetching PO detail should succeed");
        var poDetail = await ReadAs<JsonElement>(poDetailRes);

        var poStatus = poDetail.GetProperty("status").GetString();
        poStatus.Should().Be("Approved", "PO status should be Approved after approval");

        // Extract PO item IDs for goods receipt
        var poItems = poDetail.GetProperty("items");
        var poItemIds = new List<string>();
        foreach (var item in poItems.EnumerateArray())
        {
            poItemIds.Add(item.GetProperty("id").GetString()!);
        }
        poItemIds.Should().HaveCount(2, "PO should have 2 line items");

        // --- Login as warehouse user ---
        var whToken = await LoginAsync("warehouse@niagaone.com", "Warehouse@123");
        whToken.Should().NotBeNullOrWhiteSpace("warehouse login should return a valid token");

        // --- Create Goods Receipt ---
        var receiptItems = new List<object>();
        foreach (var item in poDetail.GetProperty("items").EnumerateArray())
        {
            receiptItems.Add(new
            {
                purchaseOrderItemId = item.GetProperty("id").GetString(),
                productVariantId = item.GetProperty("productVariantId").GetString(),
                quantityReceived = item.GetProperty("quantity").GetInt32()
            });
        }

        var grRes = await AuthPost(whToken, "/api/goods-receipts", new
        {
            purchaseOrderId = poId,
            warehouseId,
            notes = "Full receipt of Galaxy S24 stock",
            items = receiptItems
        });
        grRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating goods receipt should succeed");
        var gr = await ReadAs<JsonElement>(grRes);
        var grId = gr.GetProperty("id").GetString()!;
        TestDataStore.Set("goodsReceiptId", grId);

        // --- Confirm Goods Receipt ---
        var confirmRes = await AuthPost(whToken, $"/api/goods-receipts/{grId}/confirm");
        confirmRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.OK, HttpStatusCode.NoContent },
            "confirming goods receipt should succeed");

        // --- Verify Warehouse Stock ---
        var stockRes = await AuthGet(whToken, $"/api/warehouse-stock?warehouseId={warehouseId}&page=1&pageSize=50");
        stockRes.StatusCode.Should().Be(HttpStatusCode.OK, "fetching warehouse stock should succeed");
        var stockContent = await stockRes.Content.ReadAsStringAsync();
        stockContent.Should().NotBeNullOrEmpty("warehouse stock response should contain data");

        // Stock should exist after goods receipt confirmation
        var stockData = JsonSerializer.Deserialize<JsonElement>(stockContent, JsonOptions);
        // Verify stock entries exist - either as paged result or array
        if (stockData.ValueKind == JsonValueKind.Object && stockData.TryGetProperty("items", out var stockItems))
        {
            stockItems.GetArrayLength().Should().BeGreaterOrEqualTo(1,
                "warehouse should have stock after goods receipt");
        }
        else if (stockData.ValueKind == JsonValueKind.Array)
        {
            stockData.GetArrayLength().Should().BeGreaterOrEqualTo(1,
                "warehouse should have stock after goods receipt");
        }
    }
}
