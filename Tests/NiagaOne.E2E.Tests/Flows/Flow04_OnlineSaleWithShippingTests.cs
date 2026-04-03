using System.Net;
using System.Text.Json;
using FluentAssertions;
using NiagaOne.E2E.Tests.Helpers;

namespace NiagaOne.E2E.Tests.Flows;

public class Flow04_OnlineSaleWithShippingTests : NiagaOneTestBase
{
    [Fact]
    public async Task Manager_Should_Process_Online_Order_With_Shipping()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];

        // --- Login as manager ---
        var managerToken = await LoginAsync("manager@niagaone.com", "Manager@123");
        managerToken.Should().NotBeNullOrWhiteSpace("manager login should return a valid token");

        // --- Create Company Customer with Address ---
        var customerRes = await AuthPost(managerToken, "/api/customers", new
        {
            name = $"PT Maju Bersama-{suffix}",
            customerType = "Company",
            email = $"maju-{suffix}@company.co.id",
            phone = "021-7778888",
            address = "Jl. Gatot Subroto Kav.21, Jakarta Selatan 12930",
            city = "Jakarta",
            province = "DKI Jakarta",
            postalCode = "12930",
            status = "Active"
        });
        customerRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating company customer should succeed");
        var customer = await ReadAs<JsonElement>(customerRes);
        var companyCustomerId = customer.GetProperty("id").GetString()!;
        TestDataStore.Set("companyCustomerId", companyCustomerId);

        // --- Get IDs from TestDataStore ---
        var branchId = TestDataStore.Get<string>("branchId");
        var galaxy256Id = TestDataStore.Get<string>("galaxy256VariantId");
        var warehouseId = TestDataStore.Get<string>("warehouseId");

        // --- Create Online Sales Order ---
        var orderRes = await AuthPost(managerToken, "/api/sales-orders", new
        {
            customerId = companyCustomerId,
            branchId,
            orderType = "Online",
            shippingAddress = "Jl. Gatot Subroto Kav.21, Jakarta Selatan 12930",
            notes = "Online order with shipping",
            items = new[]
            {
                new { productVariantId = galaxy256Id, quantity = 5, unitPrice = 14999000m }
            }
        });
        orderRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating online sales order should succeed");
        var order = await ReadAs<JsonElement>(orderRes);
        var onlineOrderId = order.GetProperty("id").GetString()!;
        TestDataStore.Set("onlineSalesOrderId", onlineOrderId);

        // --- Confirm Sales Order ---
        var confirmRes = await AuthPost(managerToken, $"/api/sales-orders/{onlineOrderId}/confirm");
        confirmRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.OK, HttpStatusCode.NoContent },
            "confirming online sales order should succeed");

        // --- Get Grand Total and Add Bank Transfer Payment ---
        var orderDetailRes = await AuthGet(managerToken, $"/api/sales-orders/{onlineOrderId}");
        orderDetailRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var orderDetail = await ReadAs<JsonElement>(orderDetailRes);
        var grandTotal = orderDetail.GetProperty("grandTotal").GetDecimal();

        var paymentRes = await AuthPost(managerToken, $"/api/sales-orders/{onlineOrderId}/payments", new
        {
            paymentMethod = "BankTransfer",
            amount = grandTotal,
            referenceNumber = $"TRF-{suffix}",
            notes = "Bank transfer payment"
        });
        paymentRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "adding bank transfer payment should succeed");

        // --- Create Shipment ---
        var shipmentRes = await AuthPost(managerToken, "/api/shipments", new
        {
            salesOrderId = onlineOrderId,
            warehouseId,
            recipientName = $"PT Maju Bersama-{suffix}",
            recipientAddress = "Jl. Gatot Subroto Kav.21, Jakarta Selatan 12930",
            recipientPhone = "021-7778888",
            courierName = "JNE",
            trackingNumber = $"JNE-{suffix}",
            notes = "Handle with care - electronics"
        });
        shipmentRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating shipment should succeed");
        var shipment = await ReadAs<JsonElement>(shipmentRes);
        var shipmentId = shipment.GetProperty("id").GetString()!;
        TestDataStore.Set("shipmentId", shipmentId);

        // --- Add Tracking Events ---
        var pickedUpRes = await AuthPost(managerToken, $"/api/shipments/{shipmentId}/tracking-events", new
        {
            status = "PickedUp",
            location = "Gudang Utama Jakarta",
            notes = "Package picked up from warehouse",
            eventDate = DateTime.UtcNow.AddHours(-2).ToString("o")
        });
        pickedUpRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "adding PickedUp tracking event should succeed");

        var inTransitRes = await AuthPost(managerToken, $"/api/shipments/{shipmentId}/tracking-events", new
        {
            status = "InTransit",
            location = "Hub Sorting Center Cakung",
            notes = "Package in transit to destination",
            eventDate = DateTime.UtcNow.AddHours(-1).ToString("o")
        });
        inTransitRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "adding InTransit tracking event should succeed");

        var deliveredRes = await AuthPost(managerToken, $"/api/shipments/{shipmentId}/tracking-events", new
        {
            status = "Delivered",
            location = "Jl. Gatot Subroto Kav.21, Jakarta Selatan",
            notes = "Package delivered to recipient",
            eventDate = DateTime.UtcNow.ToString("o")
        });
        deliveredRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "adding Delivered tracking event should succeed");

        // --- Verify Shipment Status ---
        var shipmentDetailRes = await AuthGet(managerToken, $"/api/shipments/{shipmentId}");
        shipmentDetailRes.StatusCode.Should().Be(HttpStatusCode.OK,
            "fetching shipment detail should succeed");
        var shipmentDetail = await ReadAs<JsonElement>(shipmentDetailRes);
        var shipmentStatus = shipmentDetail.GetProperty("status").GetString();
        shipmentStatus.Should().Be("Delivered",
            "shipment status should be Delivered after all tracking events");
    }
}
