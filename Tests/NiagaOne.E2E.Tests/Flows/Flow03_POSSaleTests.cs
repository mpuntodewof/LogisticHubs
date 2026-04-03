using System.Net;
using System.Text.Json;
using FluentAssertions;
using NiagaOne.E2E.Tests.Helpers;

namespace NiagaOne.E2E.Tests.Flows;

public class Flow03_POSSaleTests : NiagaOneTestBase
{
    [Fact]
    public async Task Cashier_Should_Complete_POS_Sale()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];

        // --- Login as cashier ---
        var cashierToken = await LoginAsync("cashier@niagaone.com", "Cashier@123");
        cashierToken.Should().NotBeNullOrWhiteSpace("cashier login should return a valid token");

        // --- Create Customer ---
        var customerRes = await AuthPost(cashierToken, "/api/customers", new
        {
            name = $"Siti Rahayu-{suffix}",
            customerType = "Individual",
            email = $"siti-{suffix}@email.com",
            phone = "0812-3456-7890",
            address = "Jl. Melati No.5, Jakarta Selatan",
            status = "Active"
        });
        customerRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating customer should succeed");
        var customer = await ReadAs<JsonElement>(customerRes);
        var customerId = customer.GetProperty("id").GetString()!;
        TestDataStore.Set("customerId", customerId);

        // --- Get IDs from TestDataStore ---
        var branchId = TestDataStore.Get<string>("branchId");
        var galaxy128Id = TestDataStore.Get<string>("galaxy128VariantId");
        var nikeRedMId = TestDataStore.Get<string>("nikeRedMVariantId");

        // --- Create POS Sales Order ---
        var orderRes = await AuthPost(cashierToken, "/api/sales-orders", new
        {
            customerId,
            branchId,
            orderType = "POS",
            notes = "Walk-in POS sale",
            items = new[]
            {
                new { productVariantId = galaxy128Id, quantity = 2, unitPrice = 12999000m },
                new { productVariantId = nikeRedMId, quantity = 1, unitPrice = 299000m }
            }
        });
        orderRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating POS sales order should succeed");
        var order = await ReadAs<JsonElement>(orderRes);
        var salesOrderId = order.GetProperty("id").GetString()!;
        TestDataStore.Set("salesOrderId", salesOrderId);

        // --- Confirm Sales Order ---
        var confirmRes = await AuthPost(cashierToken, $"/api/sales-orders/{salesOrderId}/confirm");
        confirmRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.OK, HttpStatusCode.NoContent },
            "confirming sales order should succeed");

        // --- Get Order to retrieve grandTotal ---
        var orderDetailRes = await AuthGet(cashierToken, $"/api/sales-orders/{salesOrderId}");
        orderDetailRes.StatusCode.Should().Be(HttpStatusCode.OK, "fetching sales order detail should succeed");
        var orderDetail = await ReadAs<JsonElement>(orderDetailRes);
        var grandTotal = orderDetail.GetProperty("grandTotal").GetDecimal();
        grandTotal.Should().BeGreaterThan(0, "grand total should be a positive amount");
        TestDataStore.Set("posGrandTotal", grandTotal);

        // --- Add Payment ---
        var paymentRes = await AuthPost(cashierToken, $"/api/sales-orders/{salesOrderId}/payments", new
        {
            paymentMethod = "Cash",
            amount = grandTotal,
            notes = "Full cash payment"
        });
        paymentRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "adding cash payment should succeed");

        // --- Verify final order state ---
        var finalOrderRes = await AuthGet(cashierToken, $"/api/sales-orders/{salesOrderId}");
        finalOrderRes.StatusCode.Should().Be(HttpStatusCode.OK,
            "fetching final sales order should succeed");
    }
}
