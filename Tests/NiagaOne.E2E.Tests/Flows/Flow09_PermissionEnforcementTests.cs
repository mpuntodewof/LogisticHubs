using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using FluentAssertions;
using NiagaOne.E2E.Tests.Helpers;

namespace NiagaOne.E2E.Tests.Flows;

[Collection("E2E")]
public class Flow09_PermissionEnforcementTests : NiagaOneTestBase
{
    [Fact]
    public async Task Cashier_Cannot_Access_ChartOfAccounts()
    {
        var token = await LoginAsync("cashier@niagaone.com", "Password123!");

        var response = await AuthGet(token, "/api/chart-of-accounts");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Driver_Cannot_Create_Products()
    {
        var token = await LoginAsync("driver@niagaone.com", "Password123!");

        var response = await AuthPost(token, "/api/products", new
        {
            name = "Unauthorized Product",
            sku = "UNAUTH-001",
            price = 10000
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Accountant_Cannot_Manage_Employees()
    {
        var token = await LoginAsync("accountant@niagaone.com", "Password123!");

        var response = await AuthPost(token, "/api/employees", new
        {
            position = "Ghost Employee",
            hireDate = "2025-01-01",
            baseSalary = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Unauthenticated_Gets_401()
    {
        // Send request without any authorization header
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/products");
        request.Headers.Add("X-Tenant-Id", DefaultTenantId);

        var response = await Client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Viewer_Cannot_Delete_SalesOrders()
    {
        var token = await LoginAsync("viewer@niagaone.com", "Password123!");

        var randomId = Guid.NewGuid();
        var response = await AuthDelete(token, $"/api/sales-orders/{randomId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
