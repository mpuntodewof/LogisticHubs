using System.Net;
using System.Text.Json;
using FluentAssertions;
using StockLedger.E2E.Tests.Helpers;

namespace StockLedger.E2E.Tests.Flows;

public class Flow07_ReportsAndDashboardTests : StockLedgerTestBase
{
    [Fact]
    public async Task Dashboard_ReturnsValidSummary()
    {
        var token = await LoginAsync("admin@stockledger.test", "Admin123!@#");

        var resp = await AuthGet(token, $"{V1}/reports/dashboard");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var dashboard = await ReadAs<JsonElement>(resp);

        // Stock health section should exist
        var stockHealth = dashboard.GetProperty("stockHealth");
        stockHealth.GetProperty("totalSkus").GetInt32().Should().BeGreaterThanOrEqualTo(0);
        stockHealth.GetProperty("warehouseCount").GetInt32().Should().BeGreaterThanOrEqualTo(0);

        // Sales performance section should exist
        var sales = dashboard.GetProperty("salesPerformance");
        sales.GetProperty("thisMonthUnitsSold").GetInt32().Should().BeGreaterThanOrEqualTo(0);

        // Finance section should exist
        var finance = dashboard.GetProperty("finance");
        finance.GetProperty("outstandingInvoices").GetInt32().Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task ProfitAndLoss_ReturnsReportWithDateRange()
    {
        var token = await LoginAsync("admin@stockledger.test", "Admin123!@#");

        var from = DateTime.UtcNow.AddMonths(-3).ToString("yyyy-MM-dd");
        var to = DateTime.UtcNow.ToString("yyyy-MM-dd");

        var resp = await AuthGet(token, $"{V1}/reports/profit-and-loss?from={from}&to={to}");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var report = await ReadAs<JsonElement>(resp);
        report.GetProperty("fromDate").GetString().Should().NotBeNullOrEmpty();
        report.GetProperty("toDate").GetString().Should().NotBeNullOrEmpty();
        report.GetProperty("totalRevenue").GetDecimal().Should().BeGreaterThanOrEqualTo(0);
        report.GetProperty("netProfit").ValueKind.Should().NotBe(JsonValueKind.Undefined);
    }

    [Fact]
    public async Task Export_Invoices_ReturnsCsvFile()
    {
        var token = await LoginAsync("admin@stockledger.test", "Admin123!@#");

        var resp = await AuthGet(token, $"{V1}/export/invoices");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        resp.Content.Headers.ContentType?.MediaType.Should().Be("text/csv");

        var content = await resp.Content.ReadAsStringAsync();
        content.Should().Contain("InvoiceNumber");
    }

    [Fact]
    public async Task Export_Products_ReturnsCsvFile()
    {
        var token = await LoginAsync("admin@stockledger.test", "Admin123!@#");

        var resp = await AuthGet(token, $"{V1}/export/products");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await resp.Content.ReadAsStringAsync();
        content.Should().Contain("Name,Category,Brand");
    }

    [Fact]
    public async Task Notifications_ReturnsValidResponse()
    {
        var token = await LoginAsync("admin@stockledger.test", "Admin123!@#");

        var resp = await AuthGet(token, $"{V1}/notifications");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var notifications = await ReadAs<JsonElement>(resp);
        notifications.GetProperty("totalUnread").GetInt32().Should().BeGreaterThanOrEqualTo(0);
        notifications.GetProperty("notifications").GetArrayLength().Should().BeGreaterThanOrEqualTo(0);
    }
}
