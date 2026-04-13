using System.Text.Json.Serialization;
using API.Filters;
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
builder.Services.AddControllers(opts =>
    {
        opts.Filters.Add<ApiExceptionFilter>();
    })
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Swagger with JWT Bearer support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "StockLedger API", Version = "v1" });

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
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "stockledger-api",
            ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "stockledger-client",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization: register a policy per permission
string[] allPermissions =
[
    // Users & RBAC
    "users.create", "users.read", "users.update", "users.delete",
    "roles.create", "roles.read", "roles.update", "roles.delete", "roles.assign",
    // Catalog
    "categories.create", "categories.read", "categories.update", "categories.delete",
    "brands.create", "brands.read", "brands.update", "brands.delete",
    "units.create", "units.read", "units.update", "units.delete",
    "products.create", "products.read", "products.update", "products.delete",
    // Inventory
    "inventory.read", "inventory.create", "inventory.update", "inventory.transfer",
    "warehouses.manage",
    // Finance
    "chart-of-accounts.create", "chart-of-accounts.read", "chart-of-accounts.update", "chart-of-accounts.delete",
    "journal-entries.create", "journal-entries.read", "journal-entries.post", "journal-entries.void", "journal-entries.delete",
    "payment-terms.create", "payment-terms.read", "payment-terms.update", "payment-terms.delete",
    // Tax & Invoices
    "tax-rates.create", "tax-rates.read", "tax-rates.update", "tax-rates.delete", "tax-rates.assign",
    "invoices.create", "invoices.read", "invoices.issue", "invoices.assign-tax-number", "invoices.pay", "invoices.cancel", "invoices.delete",
    // Audit
    "audit-logs.read", "audit-logs.export",
    "system-logs.read",
    // Settings
    "tenant-settings.read", "tenant-settings.update",
    "system-settings.read", "system-settings.update"
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
app.UseMiddleware<API.Middleware.IdempotencyMiddleware>();
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
