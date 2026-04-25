using Domain.Interfaces;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace StockLedger.Unit.Tests.Persistence;

/// <summary>
/// Guards multi-tenant data isolation. Every entity that implements
/// <see cref="ITenantScoped"/> MUST have an EF Core query filter applied —
/// otherwise a missing <c>.Where(x =&gt; x.TenantId == ctx.TenantId)</c>
/// in any new repository would silently leak cross-tenant data.
///
/// If this test fails, do NOT delete it. Add the missing
/// <c>HasQueryFilter</c> in <see cref="AppDbContext.OnModelCreating"/>
/// using the same pattern as the surrounding entities.
/// </summary>
public class TenantQueryFilterCoverageTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(
                "server=localhost;database=stockledger_test_model_only;user=test;password=test",
                new MySqlServerVersion(new Version(8, 0, 0)))
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void Every_ITenantScoped_Entity_Has_A_Query_Filter()
    {
        using var context = CreateContext();

        var missing = context.Model.GetEntityTypes()
            .Where(et => typeof(ITenantScoped).IsAssignableFrom(et.ClrType))
            .Where(et => et.GetQueryFilter() == null)
            .Select(et => et.ClrType.Name)
            .OrderBy(n => n)
            .ToList();

        missing.Should().BeEmpty(
            "every ITenantScoped entity must have HasQueryFilter applied " +
            "in AppDbContext.OnModelCreating to prevent cross-tenant data leaks. " +
            $"Missing: {string.Join(", ", missing)}");
    }

    [Fact]
    public void Every_Query_Filter_References_TenantId()
    {
        using var context = CreateContext();

        var bad = context.Model.GetEntityTypes()
            .Where(et => typeof(ITenantScoped).IsAssignableFrom(et.ClrType))
            .Where(et =>
            {
                var filter = et.GetQueryFilter();
                if (filter == null) return false;
                var body = filter.ToString();
                return !body.Contains("TenantId");
            })
            .Select(et => et.ClrType.Name)
            .OrderBy(n => n)
            .ToList();

        bad.Should().BeEmpty(
            "every ITenantScoped entity's query filter must compare against TenantId. " +
            $"Filters that don't mention TenantId: {string.Join(", ", bad)}");
    }
}
