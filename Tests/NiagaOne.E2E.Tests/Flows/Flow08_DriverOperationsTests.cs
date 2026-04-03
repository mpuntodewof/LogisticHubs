using System.Net;
using System.Text.Json;
using FluentAssertions;
using NiagaOne.E2E.Tests.Helpers;

namespace NiagaOne.E2E.Tests.Flows;

[Collection("E2E")]
public class Flow08_DriverOperationsTests : NiagaOneTestBase
{
    [Fact]
    public async Task Driver_Should_Track_Shipment_And_Add_Notes()
    {
        // ── Login as driver ──────────────────────────────────────────────
        var token = await LoginAsync("driver@niagaone.com", "Password123!");

        // ── GET shipments ────────────────────────────────────────────────
        var shipmentsResponse = await AuthGet(token, "/api/shipments");
        shipmentsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var shipments = await ReadAs<JsonElement>(shipmentsResponse);
        string shipmentId;

        // Use existing shipment or create one via admin
        if (shipments.TryGetProperty("items", out var items) && items.GetArrayLength() > 0)
        {
            shipmentId = items[0].GetProperty("id").GetString()!;
        }
        else
        {
            // Create a shipment as admin so the driver has something to work with
            var adminToken = await LoginAsync("admin@niagaone.com", "Password123!");
            var createResponse = await AuthPost(adminToken, "/api/shipments", new
            {
                origin = "Jakarta Warehouse",
                destination = "Surabaya Store",
                scheduledDate = DateTime.UtcNow.AddDays(1),
                notes = "E2E test shipment for driver operations"
            });
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await ReadAs<JsonElement>(createResponse);
            shipmentId = created.GetProperty("id").GetString()!;
        }

        shipmentId.Should().NotBeNullOrEmpty();

        // ── Add note to shipment ─────────────────────────────────────────
        var noteResponse = await AuthPost(token, $"/api/shipments/{shipmentId}/notes", new
        {
            noteType = "DriverInstruction",
            content = "Handle with care"
        });
        noteResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // ── Verify note exists ───────────────────────────────────────────
        var notesGetResponse = await AuthGet(token, $"/api/shipments/{shipmentId}/notes");
        notesGetResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var notes = await ReadAs<JsonElement>(notesGetResponse);
        notes.GetArrayLength().Should().BeGreaterThan(0);

        // ── Post tracking events ─────────────────────────────────────────
        var trackingEvents = new[]
        {
            new { status = "PickedUp", location = "Warehouse Jakarta" },
            new { status = "InTransit", location = "Highway" },
            new { status = "Delivered", location = "Customer address" }
        };

        foreach (var evt in trackingEvents)
        {
            var trackResponse = await AuthPost(token, $"/api/shipments/{shipmentId}/tracking", evt);
            trackResponse.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
        }

        // ── Verify tracking events exist ─────────────────────────────────
        var trackingGetResponse = await AuthGet(token, $"/api/shipments/{shipmentId}/tracking");
        trackingGetResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var tracking = await ReadAs<JsonElement>(trackingGetResponse);
        tracking.GetArrayLength().Should().BeGreaterOrEqualTo(3);
    }
}
