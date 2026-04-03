using System.Net;
using System.Text.Json;
using FluentAssertions;
using NiagaOne.E2E.Tests.Helpers;

namespace NiagaOne.E2E.Tests.Flows;

public class Flow01_CompleteRetailSetupTests : NiagaOneTestBase
{
    [Fact]
    public async Task Admin_Should_Setup_Complete_Retail_Store()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];

        // --- Login as admin ---
        var token = await LoginAsync("admin@niagaone.com", "Admin@123");
        token.Should().NotBeNullOrWhiteSpace("admin login should return a valid token");

        // --- Create Parent Categories ---
        var electronicsRes = await AuthPost(token, "/api/categories", new
        {
            name = $"Electronics-{suffix}",
            description = "Electronic devices and gadgets"
        });
        electronicsRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Electronics category should succeed");
        var electronics = await ReadAs<JsonElement>(electronicsRes);
        var electronicsCatId = electronics.GetProperty("id").GetString()!;
        TestDataStore.Set("electronicsCategoryId", electronicsCatId);

        var fashionRes = await AuthPost(token, "/api/categories", new
        {
            name = $"Fashion-{suffix}",
            description = "Fashion and apparel"
        });
        fashionRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Fashion category should succeed");
        var fashion = await ReadAs<JsonElement>(fashionRes);
        var fashionCatId = fashion.GetProperty("id").GetString()!;
        TestDataStore.Set("fashionCategoryId", fashionCatId);

        // --- Create Child Categories ---
        var smartphonesRes = await AuthPost(token, "/api/categories", new
        {
            name = $"Smartphones-{suffix}",
            description = "Smartphones and mobile devices",
            parentCategoryId = electronicsCatId
        });
        smartphonesRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Smartphones sub-category should succeed");
        var smartphones = await ReadAs<JsonElement>(smartphonesRes);
        var smartphonesCatId = smartphones.GetProperty("id").GetString()!;
        TestDataStore.Set("smartphonesCategoryId", smartphonesCatId);

        var tshirtsRes = await AuthPost(token, "/api/categories", new
        {
            name = $"T-Shirts-{suffix}",
            description = "T-Shirts and casual wear",
            parentCategoryId = fashionCatId
        });
        tshirtsRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating T-Shirts sub-category should succeed");
        var tshirts = await ReadAs<JsonElement>(tshirtsRes);
        var tshirtsCatId = tshirts.GetProperty("id").GetString()!;
        TestDataStore.Set("tshirtsCategoryId", tshirtsCatId);

        // --- Create Brands ---
        var samsungBrandRes = await AuthPost(token, "/api/brands", new
        {
            name = $"Samsung-{suffix}",
            description = "Samsung Electronics"
        });
        samsungBrandRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Samsung brand should succeed");
        var samsungBrand = await ReadAs<JsonElement>(samsungBrandRes);
        var samsungBrandId = samsungBrand.GetProperty("id").GetString()!;
        TestDataStore.Set("samsungBrandId", samsungBrandId);

        var nikeBrandRes = await AuthPost(token, "/api/brands", new
        {
            name = $"Nike-{suffix}",
            description = "Nike Inc."
        });
        nikeBrandRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Nike brand should succeed");
        var nikeBrand = await ReadAs<JsonElement>(nikeBrandRes);
        var nikeBrandId = nikeBrand.GetProperty("id").GetString()!;
        TestDataStore.Set("nikeBrandId", nikeBrandId);

        // --- Create Units ---
        var pieceRes = await AuthPost(token, "/api/units", new
        {
            name = $"Piece-{suffix}",
            abbreviation = $"pcs-{suffix}"[..10]
        });
        pieceRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Piece unit should succeed");
        var piece = await ReadAs<JsonElement>(pieceRes);
        var pieceUnitId = piece.GetProperty("id").GetString()!;
        TestDataStore.Set("pieceUnitId", pieceUnitId);

        var boxRes = await AuthPost(token, "/api/units", new
        {
            name = $"Box-{suffix}",
            abbreviation = $"box-{suffix}"[..10]
        });
        boxRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Box unit should succeed");
        var box = await ReadAs<JsonElement>(boxRes);
        var boxUnitId = box.GetProperty("id").GetString()!;
        TestDataStore.Set("boxUnitId", boxUnitId);

        // --- Create Products ---
        var galaxyRes = await AuthPost(token, "/api/products", new
        {
            name = $"Samsung Galaxy S24-{suffix}",
            sku = $"SGS24-{suffix}",
            categoryId = smartphonesCatId,
            brandId = samsungBrandId,
            unitId = pieceUnitId,
            description = "Samsung Galaxy S24 flagship smartphone",
            status = "Active"
        });
        galaxyRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Samsung Galaxy S24 product should succeed");
        var galaxy = await ReadAs<JsonElement>(galaxyRes);
        var galaxyProductId = galaxy.GetProperty("id").GetString()!;
        TestDataStore.Set("galaxyProductId", galaxyProductId);

        var nikeTshirtRes = await AuthPost(token, "/api/products", new
        {
            name = $"Nike T-Shirt-{suffix}",
            sku = $"NKTS-{suffix}",
            categoryId = tshirtsCatId,
            brandId = nikeBrandId,
            unitId = pieceUnitId,
            description = "Nike casual T-Shirt",
            status = "Active"
        });
        nikeTshirtRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Nike T-Shirt product should succeed");
        var nikeTshirt = await ReadAs<JsonElement>(nikeTshirtRes);
        var nikeProductId = nikeTshirt.GetProperty("id").GetString()!;
        TestDataStore.Set("nikeProductId", nikeProductId);

        // --- Create Product Variants ---
        var galaxy128Res = await AuthPost(token, $"/api/products/{galaxyProductId}/variants", new
        {
            name = "128GB Black",
            sku = $"SGS24-128-{suffix}",
            costPrice = 8000000m,
            sellingPrice = 12999000m,
            status = "Active"
        });
        galaxy128Res.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Galaxy S24 128GB variant should succeed");
        var galaxy128 = await ReadAs<JsonElement>(galaxy128Res);
        var galaxy128Id = galaxy128.GetProperty("id").GetString()!;
        TestDataStore.Set("galaxy128VariantId", galaxy128Id);

        var galaxy256Res = await AuthPost(token, $"/api/products/{galaxyProductId}/variants", new
        {
            name = "256GB Black",
            sku = $"SGS24-256-{suffix}",
            costPrice = 9000000m,
            sellingPrice = 14999000m,
            status = "Active"
        });
        galaxy256Res.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Galaxy S24 256GB variant should succeed");
        var galaxy256 = await ReadAs<JsonElement>(galaxy256Res);
        var galaxy256Id = galaxy256.GetProperty("id").GetString()!;
        TestDataStore.Set("galaxy256VariantId", galaxy256Id);

        var nikeRedMRes = await AuthPost(token, $"/api/products/{nikeProductId}/variants", new
        {
            name = "Red M",
            sku = $"NKTS-RM-{suffix}",
            costPrice = 100000m,
            sellingPrice = 299000m,
            status = "Active"
        });
        nikeRedMRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Nike T-Shirt Red M variant should succeed");
        var nikeRedM = await ReadAs<JsonElement>(nikeRedMRes);
        var nikeRedMId = nikeRedM.GetProperty("id").GetString()!;
        TestDataStore.Set("nikeRedMVariantId", nikeRedMId);

        var nikeBlueMRes = await AuthPost(token, $"/api/products/{nikeProductId}/variants", new
        {
            name = "Blue M",
            sku = $"NKTS-BM-{suffix}",
            costPrice = 100000m,
            sellingPrice = 299000m,
            status = "Active"
        });
        nikeBlueMRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Nike T-Shirt Blue M variant should succeed");
        var nikeBlueM = await ReadAs<JsonElement>(nikeBlueMRes);
        var nikeBlueMId = nikeBlueM.GetProperty("id").GetString()!;
        TestDataStore.Set("nikeBlueMVariantId", nikeBlueMId);

        // --- Create Warehouse ---
        var warehouseRes = await AuthPost(token, "/api/warehouses", new
        {
            name = $"Gudang Utama Jakarta-{suffix}",
            code = $"GUJ-{suffix}"[..10],
            address = "Jl. Raya Industri No.1, Jakarta Utara",
            status = "Active"
        });
        warehouseRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating warehouse should succeed");
        var warehouse = await ReadAs<JsonElement>(warehouseRes);
        var warehouseId = warehouse.GetProperty("id").GetString()!;
        TestDataStore.Set("warehouseId", warehouseId);

        // --- Create Branch ---
        var branchRes = await AuthPost(token, "/api/branches", new
        {
            name = $"Jakarta Store-{suffix}",
            code = $"JKT-01-{suffix}"[..12],
            warehouseId,
            address = "Jl. Sudirman No.10, Jakarta Pusat",
            status = "Active"
        });
        branchRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating branch should succeed");
        var branch = await ReadAs<JsonElement>(branchRes);
        var branchId = branch.GetProperty("id").GetString()!;
        TestDataStore.Set("branchId", branchId);

        // --- Create Delivery Zones ---
        var jabRes = await AuthPost(token, "/api/delivery-zones", new
        {
            name = $"JABODETABEK-{suffix}",
            code = $"JABD-{suffix}"[..10],
            description = "Jakarta, Bogor, Depok, Tangerang, Bekasi"
        });
        jabRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating JABODETABEK delivery zone should succeed");
        var jab = await ReadAs<JsonElement>(jabRes);
        var jabZoneId = jab.GetProperty("id").GetString()!;
        TestDataStore.Set("jabodetabekZoneId", jabZoneId);

        var jatimRes = await AuthPost(token, "/api/delivery-zones", new
        {
            name = $"JATIM-{suffix}",
            code = $"JATM-{suffix}"[..10],
            description = "Jawa Timur region"
        });
        jatimRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating JATIM delivery zone should succeed");
        var jatim = await ReadAs<JsonElement>(jatimRes);
        var jatimZoneId = jatim.GetProperty("id").GetString()!;
        TestDataStore.Set("jatimZoneId", jatimZoneId);

        // --- Create Tax Rate ---
        var taxRes = await AuthPost(token, "/api/tax-rates", new
        {
            name = $"PPN 11%-{suffix}",
            code = $"PPN11-{suffix}"[..10],
            taxType = "PPN",
            rate = 11.0m,
            status = "Active"
        });
        taxRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating PPN 11% tax rate should succeed");
        var tax = await ReadAs<JsonElement>(taxRes);
        var taxRateId = tax.GetProperty("id").GetString()!;
        TestDataStore.Set("taxRateId", taxRateId);

        // --- Create Payment Terms ---
        var codRes = await AuthPost(token, "/api/payment-terms", new
        {
            name = $"COD-{suffix}",
            code = $"COD-{suffix}"[..10],
            dueDays = 0,
            description = "Cash on Delivery"
        });
        codRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating COD payment term should succeed");
        var cod = await ReadAs<JsonElement>(codRes);
        var codTermId = cod.GetProperty("id").GetString()!;
        TestDataStore.Set("codPaymentTermId", codTermId);

        var net30Res = await AuthPost(token, "/api/payment-terms", new
        {
            name = $"NET30-{suffix}",
            code = $"N30-{suffix}"[..10],
            dueDays = 30,
            description = "Net 30 days"
        });
        net30Res.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating NET30 payment term should succeed");
        var net30 = await ReadAs<JsonElement>(net30Res);
        var net30TermId = net30.GetProperty("id").GetString()!;
        TestDataStore.Set("net30PaymentTermId", net30TermId);

        // --- Create Customer Groups ---
        var regularRes = await AuthPost(token, "/api/customer-groups", new
        {
            name = $"Regular-{suffix}",
            discountPercent = 0m,
            description = "Regular customers, no discount"
        });
        regularRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Regular customer group should succeed");
        var regular = await ReadAs<JsonElement>(regularRes);
        var regularGroupId = regular.GetProperty("id").GetString()!;
        TestDataStore.Set("regularCustomerGroupId", regularGroupId);

        var vipRes = await AuthPost(token, "/api/customer-groups", new
        {
            name = $"VIP-{suffix}",
            discountPercent = 5m,
            description = "VIP customers, 5% discount"
        });
        vipRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating VIP customer group should succeed");
        var vip = await ReadAs<JsonElement>(vipRes);
        var vipGroupId = vip.GetProperty("id").GetString()!;
        TestDataStore.Set("vipCustomerGroupId", vipGroupId);

        // --- Verify Products ---
        var productsRes = await AuthGet(token, "/api/products?page=1&pageSize=50");
        productsRes.StatusCode.Should().Be(HttpStatusCode.OK, "fetching products should succeed");
        var productsPage = await ReadAs<PagedResult<JsonElement>>(productsRes);
        productsPage.TotalCount.Should().BeGreaterOrEqualTo(2,
            "there should be at least 2 products after retail setup");
    }
}
