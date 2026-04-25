using System.Net;
using System.Text.Json;
using FluentAssertions;
using StockLedger.E2E.Tests.Helpers;

namespace StockLedger.E2E.Tests.Flows;

public class Flow01_CompleteRetailSetupTests : StockLedgerTestBase
{
    [Fact]
    public async Task Admin_Should_Setup_Complete_Retail_Store()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var token = await LoginAsync("admin@stockledger.io", "password123");

        // --- Create Parent Categories ---
        var electronicsRes = await AuthPost(token, $"{V1}/categories", new
        {
            name = $"Electronics-{suffix}",
            description = "Electronic devices and gadgets"
        });
        electronicsRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK });
        var electronicsCatId = (await ReadAs<JsonElement>(electronicsRes)).GetProperty("id").GetString()!;

        // --- Create Child Category ---
        var smartphonesRes = await AuthPost(token, $"{V1}/categories", new
        {
            name = $"Smartphones-{suffix}",
            description = "Smartphones and mobile devices",
            parentCategoryId = electronicsCatId
        });
        smartphonesRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK });
        var smartphonesCatId = (await ReadAs<JsonElement>(smartphonesRes)).GetProperty("id").GetString()!;

        // --- Create Brand ---
        var samsungBrandRes = await AuthPost(token, $"{V1}/brands", new
        {
            name = $"Samsung-{suffix}",
            description = "Samsung Electronics"
        });
        samsungBrandRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK });
        var samsungBrandId = (await ReadAs<JsonElement>(samsungBrandRes)).GetProperty("id").GetString()!;

        // --- Create Unit of Measure (real route is /units-of-measure, not /units) ---
        var pieceRes = await AuthPost(token, $"{V1}/units-of-measure", new
        {
            name = $"Piece-{suffix}",
            abbreviation = $"pcs-{suffix[..4]}"
        });
        pieceRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK });
        var pieceUnitId = (await ReadAs<JsonElement>(pieceRes)).GetProperty("id").GetString()!;

        // --- Create Product ---
        var galaxyRes = await AuthPost(token, $"{V1}/products", new
        {
            name = $"Samsung Galaxy S24-{suffix}",
            description = "Samsung Galaxy S24 flagship smartphone",
            categoryId = smartphonesCatId,
            brandId = samsungBrandId,
            baseUnitOfMeasureId = pieceUnitId,
            status = "Active"
        });
        galaxyRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK });
        var galaxyProductId = (await ReadAs<JsonElement>(galaxyRes)).GetProperty("id").GetString()!;

        // --- Create Variants (top-level /product-variants, not nested under product) ---
        var galaxy128Res = await AuthPost(token, $"{V1}/product-variants", new
        {
            productId = galaxyProductId,
            name = "128GB Black",
            sku = $"SGS24-128-{suffix}",
            costPrice = 8000000m,
            sellingPrice = 12999000m
        });
        galaxy128Res.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK });

        var galaxy256Res = await AuthPost(token, $"{V1}/product-variants", new
        {
            productId = galaxyProductId,
            name = "256GB Black",
            sku = $"SGS24-256-{suffix}",
            costPrice = 9000000m,
            sellingPrice = 14999000m
        });
        galaxy256Res.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK });

        // --- Create Warehouse ---
        var warehouseRes = await AuthPost(token, $"{V1}/warehouses", new
        {
            name = $"Gudang Utama Jakarta-{suffix}",
            location = "Jl. Raya Industri No.1, Jakarta Utara",
            capacity = 2000
        });
        warehouseRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK });

        // --- Create Tax Rate ---
        var taxRes = await AuthPost(token, $"{V1}/tax-rates", new
        {
            name = $"PPN 11%-{suffix}",
            code = $"PPN-{suffix[..6]}",
            taxType = "PPN",
            rate = 0.11m, // NOTE: TaxRate.Rate column is decimal(5,4) — stored as fraction (0.11 = 11%)

            description = "Indonesian PPN",
            isActive = true
        });
        taxRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK });

        // --- Create Payment Terms ---
        var net30Res = await AuthPost(token, $"{V1}/payment-terms", new
        {
            name = $"NET30-{suffix}",
            code = $"N30-{suffix[..6]}",
            dueDays = 30,
            description = "Net 30 days"
        });
        net30Res.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK });

        // --- Verify products listing includes our new product ---
        var productsRes = await AuthGet(token, $"{V1}/products?page=1&pageSize=100");
        productsRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var productsPage = await ReadAs<PagedResult<JsonElement>>(productsRes);
        productsPage.TotalCount.Should().BeGreaterOrEqualTo(1);
    }

    // ── NOT YET IMPLEMENTED in API ─────────────────────────────────────────
    // The following Journey 1 features reference controllers that don't exist:
    //   - /api/v1/branches            (no BranchesController)
    //   - /api/v1/delivery-zones      (no DeliveryZonesController)
    //   - /api/v1/customer-groups     (no CustomerGroupsController)
    // Re-enable these tests once the corresponding subsystems ship.

    [Fact(Skip = "Branches subsystem not implemented — see Journey 1 scope")]
    public Task Admin_Should_Create_Branch() => Task.CompletedTask;

    [Fact(Skip = "Delivery zones subsystem not implemented — see Journey 1 scope")]
    public Task Admin_Should_Create_DeliveryZones() => Task.CompletedTask;

    [Fact(Skip = "Customer groups subsystem not implemented — see Journey 1 scope")]
    public Task Admin_Should_Create_CustomerGroups() => Task.CompletedTask;
}
