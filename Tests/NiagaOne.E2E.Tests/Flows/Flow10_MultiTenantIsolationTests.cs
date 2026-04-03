using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using NiagaOne.E2E.Tests.Helpers;

namespace NiagaOne.E2E.Tests.Flows;

[Collection("E2E")]
public class Flow10_MultiTenantIsolationTests : NiagaOneTestBase
{
    [Fact]
    public async Task Tenant_B_Data_Is_Invisible_To_Tenant_A()
    {
        // ── Register a brand-new Tenant B ────────────────────────────────
        var uniqueSuffix = DateTime.UtcNow.Ticks;
        var tenantBEmail = $"admin-tenantb-{uniqueSuffix}@e2etest.com";

        var registerRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/register")
        {
            Content = JsonContent.Create(new
            {
                companyName = "PT Test Tenant B E2E",
                fullName = "Tenant B Admin",
                email = tenantBEmail,
                password = "Password123!",
                confirmPassword = "Password123!"
            }, options: JsonOptions)
        };
        var registerResponse = await Client.SendAsync(registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // ── Login as Tenant B admin ──────────────────────────────────────
        var loginRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/login")
        {
            Content = JsonContent.Create(new
            {
                email = tenantBEmail,
                password = "Password123!"
            }, options: JsonOptions)
        };
        var loginResponse = await Client.SendAsync(loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var tenantBToken = loginResult.GetProperty("accessToken").GetString()!;
        var tenantBId = loginResult.GetProperty("user").GetProperty("tenantId").GetString()!;
        tenantBId.Should().NotBeNullOrEmpty();

        // ── Create UoM in Tenant B (required for product creation) ───────
        var uomRequest = new HttpRequestMessage(HttpMethod.Post, "/api/units-of-measure")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(new { name = "Piece", abbreviation = "pcs" }, JsonOptions),
                Encoding.UTF8,
                "application/json")
        };
        uomRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tenantBToken);
        uomRequest.Headers.Add("X-Tenant-Id", tenantBId);

        var uomResponse = await Client.SendAsync(uomRequest);
        uomResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var uom = await ReadAs<JsonElement>(uomResponse);
        var uomId = uom.GetProperty("id").GetString()!;

        // ── Create product in Tenant B ───────────────────────────────────
        var productRequest = new HttpRequestMessage(HttpMethod.Post, "/api/products")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    name = "Tenant B Secret Product",
                    sku = $"TB-SECRET-{uniqueSuffix}",
                    price = 50000,
                    unitOfMeasureId = uomId
                }, JsonOptions),
                Encoding.UTF8,
                "application/json")
        };
        productRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tenantBToken);
        productRequest.Headers.Add("X-Tenant-Id", tenantBId);

        var productResponse = await Client.SendAsync(productRequest);
        productResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // ── Login as Tenant A admin ──────────────────────────────────────
        var tenantAToken = await LoginAsync("admin@niagaone.com", "Password123!");

        // ── Search for Tenant B's product from Tenant A ──────────────────
        var searchResponse = await AuthGet(tenantAToken, "/api/products?search=Tenant+B+Secret");
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var searchResult = await ReadAs<JsonElement>(searchResponse);

        // Assert Tenant B product is NOT visible to Tenant A
        if (searchResult.TryGetProperty("totalCount", out var totalCount))
        {
            totalCount.GetInt32().Should().Be(0,
                "Tenant A must not see Tenant B's products");
        }
        else if (searchResult.TryGetProperty("items", out var searchItems))
        {
            var productNames = new List<string>();
            foreach (var item in searchItems.EnumerateArray())
            {
                productNames.Add(item.GetProperty("name").GetString()!);
            }

            productNames.Should().NotContain("Tenant B Secret Product",
                "Tenant A must not see Tenant B's products");
        }
    }
}
