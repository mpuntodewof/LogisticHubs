using System.Net;
using System.Text.Json;
using FluentAssertions;
using StockLedger.E2E.Tests.Helpers;

namespace StockLedger.E2E.Tests.Flows;

public class Flow05_FinanceAndInvoicingTests : StockLedgerTestBase
{
    [Fact]
    public async Task Accountant_Should_Create_Chart_Of_Accounts_Invoice_And_Journal_Entry()
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        var token = await LoginAsync("accountant@stockledger.io", "password123");

        // --- Chart of Accounts ---
        var arRes = await AuthPost(token, $"{V1}/chart-of-accounts", new
        {
            accountCode = $"AR-{suffix}",
            name = $"Accounts Receivable-{suffix}",
            accountType = "Asset",
            description = "Trade accounts receivable"
        });
        arRes.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
        var arAccountId = (await ReadAs<JsonElement>(arRes)).GetProperty("id").GetString()!;

        var revenueRes = await AuthPost(token, $"{V1}/chart-of-accounts", new
        {
            accountCode = $"REV-{suffix}",
            name = $"Sales Revenue-{suffix}",
            accountType = "Revenue",
            description = "Revenue from product sales"
        });
        revenueRes.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
        var revenueAccountId = (await ReadAs<JsonElement>(revenueRes)).GetProperty("id").GetString()!;

        var taxPayableRes = await AuthPost(token, $"{V1}/chart-of-accounts", new
        {
            accountCode = $"TAX-{suffix}",
            name = $"Tax Payable-{suffix}",
            accountType = "Liability",
            description = "PPN tax payable"
        });
        taxPayableRes.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
        var taxPayableAccountId = (await ReadAs<JsonElement>(taxPayableRes)).GetProperty("id").GetString()!;

        // --- Create Invoice from a real product variant in seed data ---
        var variantsRes = await AuthGet(token, $"{V1}/product-variants?page=1&pageSize=1");
        variantsRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var variants = await ReadAs<PagedResult<JsonElement>>(variantsRes);
        variants.Items.Should().NotBeEmpty("seed data must include at least one product variant");
        var variant = variants.Items[0];
        var variantId = variant.GetProperty("id").GetString()!;
        var variantSku = variant.GetProperty("sku").GetString()!;
        var variantName = variant.GetProperty("name").GetString()!;

        var invoiceRes = await AuthPost(token, $"{V1}/invoices", new
        {
            counterpartyName = $"PT Customer-{suffix}",
            dueDate = DateTime.UtcNow.AddDays(30),
            notes = "E2E test invoice",
            items = new[]
            {
                new
                {
                    productVariantId = variantId,
                    productName = "Test Product",
                    variantName,
                    sku = variantSku,
                    quantity = 10,
                    unitPrice = 50000m,
                    discountAmount = 0m
                }
            }
        });
        invoiceRes.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
        var invoice = await ReadAs<JsonElement>(invoiceRes);
        var invoiceId = invoice.GetProperty("id").GetString()!;

        // --- Issue ---
        var issueRes = await AuthPost(token, $"{V1}/invoices/{invoiceId}/issue");
        issueRes.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

        // --- Assign tax invoice number (real route is /assign-tax-number, POST) ---
        var taxNumberRes = await AuthPost(token, $"{V1}/invoices/{invoiceId}/assign-tax-number", new
        {
            taxInvoiceNumber = $"010.000-26.{suffix}"
        });
        taxNumberRes.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

        // --- Mark paid ---
        var paidRes = await AuthPost(token, $"{V1}/invoices/{invoiceId}/mark-paid");
        paidRes.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

        // --- Create balanced journal entry ---
        // Debit AR 1,000,000; Credit Revenue 900,901; Credit Tax 99,099
        var totalDebit = 1_000_000m;
        var revenueCredit = 900_901m;
        var taxCredit = totalDebit - revenueCredit; // guarantees balance

        var journalRes = await AuthPost(token, $"{V1}/journal-entries", new
        {
            entryDate = DateTime.UtcNow,
            description = $"Record sales revenue and tax-{suffix}",
            reference = $"JV-{suffix}",
            lines = new object[]
            {
                new { accountId = arAccountId,        description = "AR - sales",  debitAmount = totalDebit,   creditAmount = 0m },
                new { accountId = revenueAccountId,   description = "Sales Revenue", debitAmount = 0m,         creditAmount = revenueCredit },
                new { accountId = taxPayableAccountId, description = "PPN Tax Payable", debitAmount = 0m,      creditAmount = taxCredit }
            }
        });
        journalRes.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
        var journal = await ReadAs<JsonElement>(journalRes);
        var journalId = journal.GetProperty("id").GetString()!;

        // --- Post ---
        var postRes = await AuthPost(token, $"{V1}/journal-entries/{journalId}/post");
        postRes.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

        // --- Verify journal is balanced ---
        var journalDetailRes = await AuthGet(token, $"{V1}/journal-entries/{journalId}");
        journalDetailRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var detail = await ReadAs<JsonElement>(journalDetailRes);

        detail.GetProperty("totalDebit").GetDecimal()
            .Should().Be(detail.GetProperty("totalCredit").GetDecimal(),
                "journal entry must have totalDebit == totalCredit");
    }
}
