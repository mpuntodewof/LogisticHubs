using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using API.Filters;
using Asp.Versioning;
using Domain.Constants;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
{

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "StockLedger")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}  {Message:lj}{NewLine}{Exception}"));

/*
------------------------------------------
Validate Required Configuration
------------------------------------------
*/

var jwtSecret = builder.Configuration["JwtSettings:SecretKey"];
if (string.IsNullOrWhiteSpace(jwtSecret))
    throw new InvalidOperationException(
        "JwtSettings:SecretKey is not configured. Set it in appsettings.Development.json, environment variables, or user-secrets.");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection is not configured. Set it in appsettings.Development.json, environment variables, or user-secrets.");

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

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
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

// Authorization: register a policy per permission. Source of truth is
// Domain.Constants.Permissions — typed constants used by both this
// registration and the [RequirePermission(...)] attributes on controllers.
builder.Services.AddAuthorization(options =>
{
    foreach (var permission in Permissions.All())
    {
        options.AddPolicy($"Permission:{permission}", policy =>
            policy.RequireAuthenticatedUser()
                  .RequireClaim("permissions", permission));
    }
});

// Infrastructure Layer (Database, Repositories, Services)
builder.Services.AddInfrastructure(builder.Configuration);

// Health Checks
builder.Services.AddHealthChecks()
    .AddMySql(connectionString, name: "mysql", tags: ["db", "ready"]);

// CORS — locked to configured origins. Fail closed in non-Development if unconfigured.
var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? [];
Log.Information("CORS allowed origins ({Count}): {Origins}", allowedOrigins.Length, string.Join(", ", allowedOrigins));
if (allowedOrigins.Length == 0 && !builder.Environment.IsDevelopment())
    throw new InvalidOperationException(
        "CorsSettings:AllowedOrigins must be configured in non-Development environments. " +
        "Set at least one origin via CorsSettings__AllowedOrigins__0.");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (allowedOrigins.Length > 0)
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        else
            policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});

// Rate Limiting
var isDev = builder.Environment.IsDevelopment();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Auth endpoints: 5 req/min per IP in prod; relaxed to 1000/min in Development
    // so E2E test runs (which burst many logins) don't trip the limiter.
    options.AddPolicy("auth", context =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = isDev ? 1000 : 5,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 2,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    // General API: 100 requests per minute per IP
    options.AddPolicy("api", context =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 4,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
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

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.FirstOrDefault());
    };
});

app.UseCors();
app.UseRateLimiter();

app.UseAuthentication();
app.UseMiddleware<API.Middleware.TenantResolutionMiddleware>();
app.UseMiddleware<API.Middleware.IdempotencyMiddleware>();
app.UseAuthorization();

app.MapControllers();

// Health check endpoints (unauthenticated — for orchestrators and load balancers)
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false // liveness: app is running, no dependency checks
});
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

/*
------------------------------------------
Database Migration (Development only)
------------------------------------------
In production, run migrations via CI/CD pipeline:
  dotnet ef database update --project Infrastructure --startup-project API
*/

if (app.Environment.IsDevelopment())
{
    var retryCount = 0;
    const int maxRetries = 5;
    while (true)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();
            break;
        }
        catch (Exception ex) when (retryCount < maxRetries)
        {
            retryCount++;
            Console.WriteLine($"Database connection attempt {retryCount}/{maxRetries} failed: {ex.Message}. Retrying in 3s...");
            await Task.Delay(3000);
        }
    }
}

app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
