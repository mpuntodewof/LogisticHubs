using System.Reflection;
using System.Text.RegularExpressions;
using Domain.Constants;
using FluentAssertions;

namespace StockLedger.Unit.Tests.Domain;

/// <summary>
/// Guards the <see cref="Permissions"/> typed-constants surface that backs both
/// authorization-policy registration in <c>Program.cs</c> and per-endpoint
/// enforcement via <c>[RequirePermission(...)]</c>.
///
/// With the typed constants in place, a typo at a controller call site is a
/// compile error — so we don't need to scan controller assemblies. We only
/// need to ensure the Permissions surface itself is well-formed.
/// </summary>
public class PermissionConstantsTests
{
    private static IReadOnlyList<string> AllDeclaredViaReflection()
    {
        return typeof(Permissions)
            .GetNestedTypes()
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToList();
    }

    [Fact]
    public void All_Returns_Same_Set_As_Reflection_Walks()
    {
        var fromReflection = AllDeclaredViaReflection().OrderBy(s => s, StringComparer.Ordinal).ToList();
        var fromAll = Permissions.All().ToList();

        fromAll.Should().BeEquivalentTo(fromReflection);
    }

    [Fact]
    public void Permissions_Have_No_Duplicates()
    {
        var all = AllDeclaredViaReflection();
        var dupes = all.GroupBy(p => p).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

        dupes.Should().BeEmpty(
            "every permission string must be unique across nested resource classes. " +
            $"Duplicates: {string.Join(", ", dupes)}");
    }

    [Fact]
    public void Every_Permission_Follows_KebabResource_DotAction_Shape()
    {
        // Resource: lower-case kebab-case (e.g. "audit-logs", "chart-of-accounts")
        // Action:   lower-case kebab-case (e.g. "read", "assign-tax-number")
        var pattern = new Regex(@"^[a-z]+(-[a-z]+)*\.[a-z]+(-[a-z]+)*$");

        var bad = Permissions.All()
            .Where(p => !pattern.IsMatch(p))
            .ToList();

        bad.Should().BeEmpty(
            "permissions must be lowercase kebab-case '<resource>.<action>'. " +
            $"Bad entries: {string.Join(", ", bad)}");
    }

    [Fact]
    public void Permission_Surface_Is_Non_Empty()
    {
        Permissions.All().Should().NotBeEmpty();
    }
}
