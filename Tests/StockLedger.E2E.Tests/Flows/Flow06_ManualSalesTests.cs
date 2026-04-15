using System.Net;
using System.Text.Json;
using FluentAssertions;
using StockLedger.E2E.Tests.Helpers;

namespace StockLedger.E2E.Tests.Flows;

public class Flow06_ManualSalesTests : StockLedgerTestBase
{
    [Fact]
    public async Task ManualSale_DeductsStockAndCreatesMovement()
    {
        var token = await LoginAsync("admin@stockledger.test", "Admin123!@#");

        // Get a warehouse with stock
        var stockResp = await AuthGet(token, $"{V1}/warehouse-stock?page=1&pageSize=5");
        stockResp.EnsureSuccessStatusCode();
        var stocks = await ReadAs<PagedResult<JsonElement>>(stockResp);

        if (stocks.Items.Count == 0)
        {
            // No stock available — skip
            return;
        }

        var stock = stocks.Items[0];
        var warehouseId = stock.GetProperty("warehouseId").GetString();
        var variantId = stock.GetProperty("productVariantId").GetString();
        var qtyBefore = stock.GetProperty("quantityOnHand").GetInt32();

        if (qtyBefore < 1)
        {
            // Not enough stock for a sale — skip
            return;
        }

        // Record manual sale
        var saleResp = await AuthPost(token, $"{V1}/stock-movements/manual-sale", new
        {
            warehouseId,
            productVariantId = variantId,
            quantity = 1,
            receiptNumber = "OFFLINE-E2E-001",
            notes = "E2E test manual sale"
        });

        saleResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var movement = await ReadAs<JsonElement>(saleResp);
        movement.GetProperty("movementType").GetString().Should().Be("Out");
        movement.GetProperty("reason").GetString().Should().Be("Sale");
        movement.GetProperty("quantity").GetInt32().Should().Be(1);
        movement.GetProperty("quantityBefore").GetInt32().Should().Be(qtyBefore);
        movement.GetProperty("quantityAfter").GetInt32().Should().Be(qtyBefore - 1);
    }

    [Fact]
    public async Task ManualSale_InsufficientStock_Returns409()
    {
        var token = await LoginAsync("admin@stockledger.test", "Admin123!@#");

        // Try to sell 999999 units — should fail
        var stockResp = await AuthGet(token, $"{V1}/warehouse-stock?page=1&pageSize=1");
        stockResp.EnsureSuccessStatusCode();
        var stocks = await ReadAs<PagedResult<JsonElement>>(stockResp);

        if (stocks.Items.Count == 0) return;

        var stock = stocks.Items[0];
        var warehouseId = stock.GetProperty("warehouseId").GetString();
        var variantId = stock.GetProperty("productVariantId").GetString();

        var saleResp = await AuthPost(token, $"{V1}/stock-movements/manual-sale", new
        {
            warehouseId,
            productVariantId = variantId,
            quantity = 999999,
            notes = "E2E test — should fail"
        });

        saleResp.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
