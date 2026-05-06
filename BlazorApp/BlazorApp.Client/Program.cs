using Blazored.LocalStorage;
using BlazorApp.Client.Services;
using BlazorApp.Client.Services.ApiClients;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// HTTP client pointing at the API
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5164";
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl.TrimEnd('/') + "/")
});

// Auth
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore(options =>
{
    var allPermissions = new[]
    {
        // Users & Roles
        "users.create", "users.read", "users.update", "users.delete",
        "roles.assign",
        "roles.create", "roles.read", "roles.update", "roles.delete",
        // Warehouses
        "warehouses.manage",
        // Catalog
        "categories.create", "categories.read", "categories.update", "categories.delete",
        "brands.create", "brands.read", "brands.update", "brands.delete",
        "units.create", "units.read", "units.update", "units.delete",
        "products.create", "products.read", "products.update", "products.delete",
        // Inventory
        "inventory.read", "inventory.create", "inventory.update", "inventory.transfer",
        // Finance
        "chart-of-accounts.create", "chart-of-accounts.read", "chart-of-accounts.update", "chart-of-accounts.delete",
        "journal-entries.create", "journal-entries.read", "journal-entries.post", "journal-entries.void", "journal-entries.delete",
        "payment-terms.create", "payment-terms.read", "payment-terms.update", "payment-terms.delete",
        // Tax & Invoices
        "tax-rates.create", "tax-rates.read", "tax-rates.update", "tax-rates.delete", "tax-rates.assign",
        "invoices.create", "invoices.read", "invoices.issue", "invoices.assign-tax-number", "invoices.pay", "invoices.cancel", "invoices.delete",
        // Reporting
        "reports.read", "reports.execute",
        // Audit
        "audit-logs.read", "audit-logs.export",
        // Settings
        "tenant-settings.read", "tenant-settings.update"
    };

    foreach (var permission in allPermissions)
    {
        options.AddPolicy($"Permission:{permission}", policy =>
            policy.RequireAuthenticatedUser()
                  .RequireClaim("permissions", permission));
    }
});
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthService>();

// Feature-scoped API clients
builder.Services.AddScoped<AuthClient>();
builder.Services.AddScoped<UserManagementClient>();
builder.Services.AddScoped<CatalogClient>();
builder.Services.AddScoped<InventoryClient>();
builder.Services.AddScoped<FinanceClient>();
builder.Services.AddScoped<ImportClient>();
builder.Services.AddScoped<ReportingClient>();
builder.Services.AddScoped<SettingsClient>();

await builder.Build().RunAsync();
