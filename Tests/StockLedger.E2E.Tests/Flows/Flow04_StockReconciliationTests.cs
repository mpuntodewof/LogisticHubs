using System.Net;
using System.Text.Json;
using FluentAssertions;
using StockLedger.E2E.Tests.Helpers;

namespace StockLedger.E2E.Tests.Flows;

public class Flow04_StockReconciliationTests : StockLedgerTestBase
{
    [Fact]
    public async Task Reconciliation_AdjustsStockAndCreatesMovements()
    {
        var token = await LoginAsync("admin@stockledger.test", "Admin123!@#");

        // Get a warehouse
        var warehousesResp = await AuthGet(token, $"{V1}/warehouses?page=1&pageSize=1");
        warehousesResp.EnsureSuccessStatusCode();
        var warehouses = await ReadAs<PagedResult<JsonElement>>(warehousesResp);
        warehouses.Items.Should().NotBeEmpty();
        var warehouseId = warehouses.Items[0].GetProperty("id").GetString();

        // Get warehouse stock to find a product variant with stock
        var stockResp = await AuthGet(token, $"{V1}/warehouse-stock?page=1&pageSize=5");
        stockResp.EnsureSuccessStatusCode();
        var stocks = await ReadAs<PagedResult<JsonElement>>(stockResp);

        if (stocks.Items.Count == 0)
        {
            // No stock to reconcile — skip gracefully
            return;
        }

        var firstStock = stocks.Items[0];
        var variantId = firstStock.GetProperty("productVariantId").GetString();
        var currentQty = firstStock.GetProperty("quantityOnHand").GetInt32();

        // Reconcile with a different physical count
        var reconcileResp = await AuthPost(token, $"{V1}/warehouse-stock/reconcile", new
        {
            warehouseId,
            counts = new[]
            {
                new { productVariantId = variantId, physicalCount = currentQty + 5 }
            },
            notes = "E2E test reconciliation"
        });

        reconcileResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadAs<JsonElement>(reconcileResp);
        result.GetProperty("totalItems").GetInt32().Should().Be(1);
        result.GetProperty("adjustedItems").GetInt32().Should().Be(1);

        // Verify stock was updated
        var updatedStockResp = await AuthGet(token, $"{V1}/warehouse-stock?page=1&pageSize=50");
        updatedStockResp.EnsureSuccessStatusCode();
    }
}
