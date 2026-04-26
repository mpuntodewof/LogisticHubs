namespace Domain.Constants
{
    /// <summary>
    /// Typed permission constants used by both authorization-policy registration
    /// (in API/Program.cs) and per-endpoint enforcement (via
    /// <c>[RequirePermission(...)]</c> on controllers).
    ///
    /// Adding a new permission: declare a <c>const string</c> here. The
    /// PermissionPolicyCoverageTests unit test ensures every declared constant
    /// has a corresponding policy registered at startup.
    ///
    /// Why nested classes: gives IntelliSense the resource grouping
    /// (<c>Permissions.Brands.Create</c>) without scattering ~60 top-level
    /// constants. Why <c>const</c> not <c>static readonly</c>: attribute
    /// arguments must be compile-time constants.
    /// </summary>
    public static class Permissions
    {
        // ── Users & RBAC ────────────────────────────────────────────────────
        public static class Users
        {
            public const string Create = "users.create";
            public const string Read = "users.read";
            public const string Update = "users.update";
            public const string Delete = "users.delete";
        }

        public static class Roles
        {
            public const string Create = "roles.create";
            public const string Read = "roles.read";
            public const string Update = "roles.update";
            public const string Delete = "roles.delete";
            public const string Assign = "roles.assign";
        }

        // ── Catalog ─────────────────────────────────────────────────────────
        public static class Categories
        {
            public const string Create = "categories.create";
            public const string Read = "categories.read";
            public const string Update = "categories.update";
            public const string Delete = "categories.delete";
        }

        public static class Brands
        {
            public const string Create = "brands.create";
            public const string Read = "brands.read";
            public const string Update = "brands.update";
            public const string Delete = "brands.delete";
        }

        public static class Units
        {
            public const string Create = "units.create";
            public const string Read = "units.read";
            public const string Update = "units.update";
            public const string Delete = "units.delete";
        }

        public static class Products
        {
            public const string Create = "products.create";
            public const string Read = "products.read";
            public const string Update = "products.update";
            public const string Delete = "products.delete";
        }

        // ── Inventory ───────────────────────────────────────────────────────
        public static class Inventory
        {
            public const string Read = "inventory.read";
            public const string Create = "inventory.create";
            public const string Update = "inventory.update";
            public const string Transfer = "inventory.transfer";
        }

        public static class Warehouses
        {
            public const string Manage = "warehouses.manage";
        }

        // ── Finance ─────────────────────────────────────────────────────────
        public static class ChartOfAccounts
        {
            public const string Create = "chart-of-accounts.create";
            public const string Read = "chart-of-accounts.read";
            public const string Update = "chart-of-accounts.update";
            public const string Delete = "chart-of-accounts.delete";
        }

        public static class JournalEntries
        {
            public const string Create = "journal-entries.create";
            public const string Read = "journal-entries.read";
            public const string Post = "journal-entries.post";
            public const string Void = "journal-entries.void";
            public const string Delete = "journal-entries.delete";
        }

        public static class PaymentTerms
        {
            public const string Create = "payment-terms.create";
            public const string Read = "payment-terms.read";
            public const string Update = "payment-terms.update";
            public const string Delete = "payment-terms.delete";
        }

        // ── Tax & Invoices ──────────────────────────────────────────────────
        public static class TaxRates
        {
            public const string Create = "tax-rates.create";
            public const string Read = "tax-rates.read";
            public const string Update = "tax-rates.update";
            public const string Delete = "tax-rates.delete";
            public const string Assign = "tax-rates.assign";
        }

        public static class Invoices
        {
            public const string Create = "invoices.create";
            public const string Read = "invoices.read";
            public const string Issue = "invoices.issue";
            public const string AssignTaxNumber = "invoices.assign-tax-number";
            public const string Pay = "invoices.pay";
            public const string Cancel = "invoices.cancel";
            public const string Delete = "invoices.delete";
        }

        // ── Audit ───────────────────────────────────────────────────────────
        public static class AuditLogs
        {
            public const string Read = "audit-logs.read";
            public const string Export = "audit-logs.export";
        }

        public static class SystemLogs
        {
            public const string Read = "system-logs.read";
        }

        // ── Settings ────────────────────────────────────────────────────────
        public static class TenantSettings
        {
            public const string Read = "tenant-settings.read";
            public const string Update = "tenant-settings.update";
        }

        public static class SystemSettings
        {
            public const string Read = "system-settings.read";
            public const string Update = "system-settings.update";
        }

        /// <summary>
        /// Returns every declared permission string by reflecting over the
        /// nested classes. Used by <c>Program.cs</c> to register one
        /// authorization policy per permission, and by tests to enforce
        /// that no controller references a permission that wasn't declared.
        /// </summary>
        public static IEnumerable<string> All()
        {
            return typeof(Permissions)
                .GetNestedTypes()
                .SelectMany(t => t.GetFields(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Static |
                    System.Reflection.BindingFlags.FlattenHierarchy))
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                .Select(f => (string)f.GetRawConstantValue()!)
                .OrderBy(s => s, StringComparer.Ordinal);
        }
    }
}
