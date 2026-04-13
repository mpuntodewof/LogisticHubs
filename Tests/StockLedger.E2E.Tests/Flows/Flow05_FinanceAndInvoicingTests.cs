using System.Net;
using System.Text.Json;
using FluentAssertions;
using StockLedger.E2E.Tests.Helpers;

namespace StockLedger.E2E.Tests.Flows;

public class Flow05_FinanceAndInvoicingTests : StockLedgerTestBase
{
    [Fact]
    public async Task Accountant_Should_Create_Invoice_And_Journal_Entry()
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];

        // --- Login as accountant ---
        var accountantToken = await LoginAsync("accountant@stockledger.io", "Accountant@123");
        accountantToken.Should().NotBeNullOrWhiteSpace("accountant login should return a valid token");

        // --- Create Chart of Accounts ---
        // 1. Accounts Receivable (Asset)
        var arRes = await AuthPost(accountantToken, "/api/chart-of-accounts", new
        {
            name = $"Accounts Receivable-{suffix}",
            code = $"1100-{suffix}"[..10],
            accountType = "Asset",
            description = "Trade accounts receivable",
            isActive = true
        });
        arRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating AR account should succeed");
        var ar = await ReadAs<JsonElement>(arRes);
        var arAccountId = ar.GetProperty("id").GetString()!;
        TestDataStore.Set("arAccountId", arAccountId);

        // 2. Revenue (Revenue)
        var revenueRes = await AuthPost(accountantToken, "/api/chart-of-accounts", new
        {
            name = $"Sales Revenue-{suffix}",
            code = $"4100-{suffix}"[..10],
            accountType = "Revenue",
            description = "Revenue from product sales",
            isActive = true
        });
        revenueRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Revenue account should succeed");
        var revenue = await ReadAs<JsonElement>(revenueRes);
        var revenueAccountId = revenue.GetProperty("id").GetString()!;
        TestDataStore.Set("revenueAccountId", revenueAccountId);

        // 3. Tax Payable (Liability)
        var taxPayableRes = await AuthPost(accountantToken, "/api/chart-of-accounts", new
        {
            name = $"Tax Payable-{suffix}",
            code = $"2100-{suffix}"[..10],
            accountType = "Liability",
            description = "PPN tax payable",
            isActive = true
        });
        taxPayableRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating Tax Payable account should succeed");
        var taxPayable = await ReadAs<JsonElement>(taxPayableRes);
        var taxPayableAccountId = taxPayable.GetProperty("id").GetString()!;
        TestDataStore.Set("taxPayableAccountId", taxPayableAccountId);

        // --- Create Invoice from Sales Order ---
        var salesOrderId = TestDataStore.Get<string>("salesOrderId");

        var invoiceRes = await AuthPost(accountantToken, "/api/invoices", new
        {
            salesOrderId,
            dueDate = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd"),
            notes = "Invoice for POS sale"
        });
        invoiceRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating invoice should succeed");
        var invoice = await ReadAs<JsonElement>(invoiceRes);
        var invoiceId = invoice.GetProperty("id").GetString()!;
        TestDataStore.Set("invoiceId", invoiceId);

        // --- Issue Invoice ---
        var issueRes = await AuthPost(accountantToken, $"/api/invoices/{invoiceId}/issue");
        issueRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.OK, HttpStatusCode.NoContent },
            "issuing invoice should succeed");

        // --- Assign Tax Number ---
        var taxNumberRes = await AuthPut(accountantToken, $"/api/invoices/{invoiceId}/tax-number", new
        {
            taxNumber = $"010.000-24.{suffix}"
        });
        taxNumberRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.OK, HttpStatusCode.NoContent },
            "assigning tax number should succeed");

        // --- Mark Invoice as Paid ---
        var paidRes = await AuthPost(accountantToken, $"/api/invoices/{invoiceId}/mark-paid", new
        {
            paymentDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            paymentMethod = "Cash",
            notes = "Payment received in full"
        });
        paidRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.OK, HttpStatusCode.NoContent },
            "marking invoice as paid should succeed");

        // --- Create Balanced Journal Entry ---
        // Total = Revenue + Tax (e.g., 26,297,000 revenue + 2,892,670 tax = 29,189,670 total)
        // Use round numbers for the test journal entry
        var totalDebit = 10000000m;
        var revenueCredit = 9009009m;   // ~90.09%
        var taxCredit = 990991m;         // ~9.91% (PPN 11% of revenue)
        // Ensure balanced: totalDebit == revenueCredit + taxCredit
        taxCredit = totalDebit - revenueCredit;

        var journalRes = await AuthPost(accountantToken, "/api/journal-entries", new
        {
            entryDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            description = $"Record sales revenue and tax-{suffix}",
            referenceNumber = $"JV-{suffix}",
            lines = new[]
            {
                new
                {
                    accountId = arAccountId,
                    debit = totalDebit,
                    credit = 0m,
                    description = "Accounts Receivable - sales"
                },
                new
                {
                    accountId = revenueAccountId,
                    debit = 0m,
                    credit = revenueCredit,
                    description = "Sales Revenue"
                },
                new
                {
                    accountId = taxPayableAccountId,
                    debit = 0m,
                    credit = taxCredit,
                    description = "PPN Tax Payable"
                }
            }
        });
        journalRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            "creating journal entry should succeed");
        var journal = await ReadAs<JsonElement>(journalRes);
        var journalId = journal.GetProperty("id").GetString()!;
        TestDataStore.Set("journalEntryId", journalId);

        // --- Post Journal Entry ---
        var postRes = await AuthPost(accountantToken, $"/api/journal-entries/{journalId}/post");
        postRes.StatusCode.Should().BeOneOf(new[] { HttpStatusCode.OK, HttpStatusCode.NoContent },
            "posting journal entry should succeed");

        // --- Verify Journal Entry Balance ---
        var journalDetailRes = await AuthGet(accountantToken, $"/api/journal-entries/{journalId}");
        journalDetailRes.StatusCode.Should().Be(HttpStatusCode.OK,
            "fetching journal entry detail should succeed");
        var journalDetail = await ReadAs<JsonElement>(journalDetailRes);

        // Verify totalDebit == totalCredit
        if (journalDetail.TryGetProperty("totalDebit", out var tdProp)
            && journalDetail.TryGetProperty("totalCredit", out var tcProp))
        {
            var actualTotalDebit = tdProp.GetDecimal();
            var actualTotalCredit = tcProp.GetDecimal();
            actualTotalDebit.Should().Be(actualTotalCredit,
                "journal entry must be balanced: totalDebit == totalCredit");
        }
        else
        {
            // If totals aren't in the response, compute from lines
            if (journalDetail.TryGetProperty("lines", out var lines))
            {
                decimal sumDebit = 0, sumCredit = 0;
                foreach (var line in lines.EnumerateArray())
                {
                    sumDebit += line.GetProperty("debit").GetDecimal();
                    sumCredit += line.GetProperty("credit").GetDecimal();
                }
                sumDebit.Should().Be(sumCredit,
                    "journal entry must be balanced: sum of debits == sum of credits");
            }
        }
    }
}
