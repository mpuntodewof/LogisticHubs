using System.Text.Json.Serialization;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/*
------------------------------------------
Register Services
------------------------------------------
*/

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Swagger with JWT Bearer support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "NiagaOne API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: eyJhbGci..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Authentication
var jwtSecret = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "niagaone-api",
            ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "niagaone-client",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization: register a policy per permission
string[] allPermissions =
[
    "users.create", "users.read", "users.update", "users.delete",
    "roles.assign",
    "shipments.create", "shipments.read", "shipments.update", "shipments.delete", "shipments.assign",
    "tracking.create", "tracking.read",
    "drivers.manage", "vehicles.manage", "warehouses.manage",
    "roles.create", "roles.read", "roles.update", "roles.delete",
    // Catalog
    "categories.create", "categories.read", "categories.update", "categories.delete",
    "brands.create", "brands.read", "brands.update", "brands.delete",
    "units.create", "units.read", "units.update", "units.delete",
    "products.create", "products.read", "products.update", "products.delete",
    // Inventory
    "inventory.read", "inventory.create", "inventory.update", "inventory.transfer",
    // CRM
    "customer-groups.create", "customer-groups.read", "customer-groups.update", "customer-groups.delete",
    "customers.create", "customers.read", "customers.update", "customers.delete",
    // Sales
    "sales-orders.create", "sales-orders.read", "sales-orders.update", "sales-orders.delete",
    "sales-orders.confirm", "sales-orders.cancel", "sales-orders.pay",
    // Multi-Branch
    "branches.create", "branches.read", "branches.update", "branches.delete", "branches.assign",
    // Finance
    "chart-of-accounts.create", "chart-of-accounts.read", "chart-of-accounts.update", "chart-of-accounts.delete",
    "journal-entries.create", "journal-entries.read", "journal-entries.post", "journal-entries.void", "journal-entries.delete",
    "payment-terms.create", "payment-terms.read", "payment-terms.update", "payment-terms.delete",
    // Tax
    "tax-rates.create", "tax-rates.read", "tax-rates.update", "tax-rates.delete", "tax-rates.assign",
    "invoices.create", "invoices.read", "invoices.issue", "invoices.assign-tax-number", "invoices.pay", "invoices.cancel", "invoices.delete",
    // Payment Gateway
    "payment-gateways.create", "payment-gateways.read", "payment-gateways.update", "payment-gateways.delete",
    "payment-transactions.create", "payment-transactions.read",
    // E-commerce
    "shopping-carts.read", "shopping-carts.manage",
    "wishlists.read", "wishlists.manage",
    "product-reviews.read", "product-reviews.moderate", "product-reviews.delete",
    "coupons.create", "coupons.read", "coupons.update", "coupons.delete",
    // Storefront
    "storefront-config.read", "storefront-config.update",
    "banners.create", "banners.read", "banners.update", "banners.delete",
    "pages.create", "pages.read", "pages.update", "pages.delete",
    // Logistics Enhancements
    "delivery-zones.create", "delivery-zones.read", "delivery-zones.update", "delivery-zones.delete",
    "delivery-rates.create", "delivery-rates.read", "delivery-rates.update", "delivery-rates.delete",
    "shipment-notes.create", "shipment-notes.read", "shipment-notes.delete",
    "sales-orders.ship",
    // Promotions
    "promotions.create", "promotions.read", "promotions.update", "promotions.delete", "promotions.activate",
    // Loyalty
    "loyalty.create", "loyalty.read", "loyalty.update", "loyalty.delete",
    "loyalty.enroll", "loyalty.adjust", "loyalty.redeem",
    // Purchase
    "suppliers.create", "suppliers.read", "suppliers.update", "suppliers.delete",
    "purchase-orders.create", "purchase-orders.read", "purchase-orders.update", "purchase-orders.delete",
    "purchase-orders.submit", "purchase-orders.approve", "purchase-orders.cancel",
    "goods-receipts.create", "goods-receipts.read", "goods-receipts.confirm", "goods-receipts.delete",
    // Notification
    "notification-templates.create", "notification-templates.read", "notification-templates.update", "notification-templates.delete",
    "notifications.read", "notifications.create", "notifications.manage",
    "notification-preferences.read", "notification-preferences.update",
    // HRM
    "departments.create", "departments.read", "departments.update", "departments.delete",
    "employees.create", "employees.read", "employees.update", "employees.delete",
    "attendance.read", "attendance.manage",
    "leave-requests.create", "leave-requests.read", "leave-requests.approve",
    // Reporting
    "reports.create", "reports.read", "reports.update", "reports.delete", "reports.execute",
    "report-executions.read",
    "dashboard-widgets.create", "dashboard-widgets.read", "dashboard-widgets.update", "dashboard-widgets.delete",
    // Audit
    "audit-logs.read", "audit-logs.export",
    "system-logs.read",
    // Settings
    "tenant-settings.read", "tenant-settings.update",
    "system-settings.read", "system-settings.update",
    "api-keys.create", "api-keys.read", "api-keys.update", "api-keys.delete", "api-keys.regenerate",
    // Webhooks
    "webhooks.create", "webhooks.read", "webhooks.update", "webhooks.delete", "webhooks.test",
    "webhook-deliveries.read", "webhook-deliveries.retry"
];

builder.Services.AddAuthorization(options =>
{
    foreach (var permission in allPermissions)
    {
        options.AddPolicy($"Permission:{permission}", policy =>
            policy.RequireAuthenticatedUser()
                  .RequireClaim("permissions", permission));
    }
});

// Infrastructure Layer (Database, Repositories, Services)
builder.Services.AddInfrastructure(builder.Configuration);

// CORS (for Blazor WebAssembly)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});

var app = builder.Build();

/*
------------------------------------------
Middleware Pipeline
------------------------------------------
*/

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Disabled — running HTTP-only in dev

app.UseCors("AllowBlazorClient");

app.UseAuthentication();
app.UseMiddleware<API.Middleware.TenantResolutionMiddleware>();
app.UseAuthorization();

app.MapControllers();

/*
------------------------------------------
Auto Apply Database Migration
------------------------------------------
*/

var retryCount = 0;
const int maxRetries = 5;
while (true)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
        break;
    }
    catch (Exception ex) when (retryCount < maxRetries)
    {
        retryCount++;
        Console.WriteLine($"Database connection attempt {retryCount}/{maxRetries} failed: {ex.Message}. Retrying in 3s...");
        Thread.Sleep(3000);
    }
}

app.Run();