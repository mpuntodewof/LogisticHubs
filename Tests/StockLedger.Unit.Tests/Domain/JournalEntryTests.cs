using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace StockLedger.Unit.Tests.Domain;

public class JournalEntryTests
{
    private static JournalEntry CreateEntry(JournalEntryStatus status = JournalEntryStatus.Draft)
    {
        return new JournalEntry
        {
            Id = Guid.NewGuid(),
            EntryNumber = "JE-001",
            EntryDate = DateTime.UtcNow,
            Status = status.ToString(),
            TotalDebit = 1000,
            TotalCredit = 1000,
            TenantId = Guid.NewGuid()
        };
    }

    [Fact]
    public void Post_FromDraft_SetsStatusAndTimestamp()
    {
        var entry = CreateEntry(JournalEntryStatus.Draft);

        entry.Post();

        entry.Status.Should().Be("Posted");
        entry.PostedAt.Should().NotBeNull();
        entry.IsPosted.Should().BeTrue();
    }

    [Fact]
    public void Post_FromPosted_Throws()
    {
        var entry = CreateEntry(JournalEntryStatus.Posted);

        var act = () => entry.Post();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Only draft*");
    }

    [Fact]
    public void Void_FromPosted_SetsStatusAndReason()
    {
        var entry = CreateEntry(JournalEntryStatus.Posted);

        entry.Void("Incorrect amounts");

        entry.Status.Should().Be("Voided");
        entry.VoidedAt.Should().NotBeNull();
        entry.VoidReason.Should().Be("Incorrect amounts");
        entry.IsVoided.Should().BeTrue();
    }

    [Fact]
    public void Void_FromDraft_Throws()
    {
        var entry = CreateEntry(JournalEntryStatus.Draft);

        var act = () => entry.Void("reason");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Only posted*");
    }

    [Fact]
    public void EnsureCanDelete_Draft_DoesNotThrow()
    {
        var entry = CreateEntry(JournalEntryStatus.Draft);

        var act = () => entry.EnsureCanDelete();

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureCanDelete_Posted_Throws()
    {
        var entry = CreateEntry(JournalEntryStatus.Posted);

        var act = () => entry.EnsureCanDelete();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Only draft*");
    }
}
