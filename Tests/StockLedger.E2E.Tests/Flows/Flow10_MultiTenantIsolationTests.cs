using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using StockLedger.E2E.Tests.Helpers;

namespace StockLedger.E2E.Tests.Flows;

public class Flow10_MultiTenantIsolationTests : StockLedgerTestBase
{
    [Fact]
    public async Task Tenant_B_Cannot_See_Default_Tenant_Products()
    {
        // Unique suffix must include both ticks AND a random guid slice — otherwise
        // re-runs collide on the tenant Slug unique index (see Bug G).
        var uniqueSuffix = $"{DateTime.UtcNow.Ticks:x}-{Guid.NewGuid().ToString("N")[..6]}";
        var tenantBEmail = $"admin-tenantb-{uniqueSuffix}@e2etest.com";
        var tenantBCompany = $"PT Test Tenant B E2E {uniqueSuffix}";

        // ── Register Tenant B ────────────────────────────────────────────
        var registerRequest = new HttpRequestMessage(HttpMethod.Post, $"{V1}/auth/register")
        {
            Content = JsonContent.Create(new
            {
                companyName = tenantBCompany,
                name = "Tenant B Admin",
                email = tenantBEmail,
                password = "Password123!"
            }, options: JsonOptions)
        };
        var registerResponse = await Client.SendAsync(registerRequest);
        registerResponse.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);

        // ── Login as Tenant B ────────────────────────────────────────────
        var loginRequest = new HttpRequestMessage(HttpMethod.Post, $"{V1}/auth/login")
        {
            Content = JsonContent.Create(new { email = tenantBEmail, password = "Password123!" }, options: JsonOptions)
        };
        var loginResponse = await Client.SendAsync(loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        var tenantBToken = loginResult.GetProperty("accessToken").GetString()!;
        var tenantBId = loginResult.GetProperty("user").GetProperty("tenantId").GetString()!;

        // ── Tenant B lists products — must NOT see default tenant's seed products ──
        var productsReq = new HttpRequestMessage(HttpMethod.Get, $"{V1}/products?page=1&pageSize=50");
        productsReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tenantBToken);
        productsReq.Headers.Add("X-Tenant-Id", tenantBId);
        var productsRes = await Client.SendAsync(productsReq);

        // The products listing may return 200 (empty) or 403 (no read permission) —
        // both outcomes satisfy the isolation contract: Tenant B cannot see
        // default-tenant seed products (IDM-GRG-001, AQU-600-001, etc.).
        if (productsRes.StatusCode == HttpStatusCode.OK)
        {
            var result = await ReadAs<PagedResult<JsonElement>>(productsRes);
            var skus = result.Items
                .Where(i => i.TryGetProperty("slug", out _))
                .Select(i => i.GetProperty("slug").GetString() ?? "")
                .ToList();
            skus.Should().NotContain("indomie-mi-goreng",
                "Tenant B must not see the default tenant's seed products");
            skus.Should().NotContain("aqua-mineral-water",
                "Tenant B must not see the default tenant's seed products");
        }
        else
        {
            productsRes.StatusCode.Should().Be(HttpStatusCode.Forbidden,
                "non-200 response for cross-tenant read must be 403 (permission), not 500 (bug)");
        }
    }

    // NOTE: Bug F — newly-registered tenants are seeded with permissions from an old
    // logistics domain (shipments/drivers/tracking) rather than StockLedger's
    // retail domain (products/inventory/units). See
    // Infrastructure/Repositories/AuthRepository.cs:SeedRolesAndPermissionsForTenantAsync.
    // Once that's fixed, expand this flow to also assert Tenant B can create its
    // own products and that Tenant A cannot see them.
}
