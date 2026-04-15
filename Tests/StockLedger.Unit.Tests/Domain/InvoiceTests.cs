using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace StockLedger.Unit.Tests.Domain;

public class InvoiceTests
{
    private static Invoice CreateInvoice(InvoiceStatus status = InvoiceStatus.Draft)
    {
        return new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = "INV-001",
            Status = status.ToString(),
            InvoiceDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            TenantId = Guid.NewGuid()
        };
    }

    [Fact]
    public void Issue_FromDraft_SetsStatusAndTimestamp()
    {
        var invoice = CreateInvoice(InvoiceStatus.Draft);

        invoice.Issue();

        invoice.Status.Should().Be("Issued");
        invoice.IssuedAt.Should().NotBeNull();
        invoice.IsIssued.Should().BeTrue();
    }

    [Fact]
    public void Issue_FromPaid_ThrowsInvalidOperation()
    {
        var invoice = CreateInvoice(InvoiceStatus.Paid);

        var act = () => invoice.Issue();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot transition*");
    }

    [Fact]
    public void MarkPaid_FromIssued_SetsStatusAndTimestamp()
    {
        var invoice = CreateInvoice(InvoiceStatus.Issued);

        invoice.MarkPaid();

        invoice.Status.Should().Be("Paid");
        invoice.PaidAt.Should().NotBeNull();
        invoice.IsPaid.Should().BeTrue();
    }

    [Fact]
    public void MarkPaid_FromDraft_ThrowsInvalidOperation()
    {
        var invoice = CreateInvoice(InvoiceStatus.Draft);

        var act = () => invoice.MarkPaid();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cancel_FromDraft_Succeeds()
    {
        var invoice = CreateInvoice(InvoiceStatus.Draft);

        invoice.Cancel("Test reason");

        invoice.Status.Should().Be("Cancelled");
        invoice.CancelledAt.Should().NotBeNull();
        invoice.CancellationReason.Should().Be("Test reason");
    }

    [Fact]
    public void Cancel_FromIssued_Succeeds()
    {
        var invoice = CreateInvoice(InvoiceStatus.Issued);

        invoice.Cancel("Late payment");

        invoice.IsCancelled.Should().BeTrue();
    }

    [Fact]
    public void Cancel_FromPaid_ThrowsInvalidOperation()
    {
        var invoice = CreateInvoice(InvoiceStatus.Paid);

        var act = () => invoice.Cancel("reason");

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void EnsureCanDelete_Draft_DoesNotThrow()
    {
        var invoice = CreateInvoice(InvoiceStatus.Draft);

        var act = () => invoice.EnsureCanDelete();

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureCanDelete_Issued_Throws()
    {
        var invoice = CreateInvoice(InvoiceStatus.Issued);

        var act = () => invoice.EnsureCanDelete();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Only draft*");
    }
}
