using Blazored.LocalStorage;
using BlazorApp.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// HTTP client pointing at the API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:59980/")
});

// Auth
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore(options =>
{
    var allPermissions = new[]
    {
        "users.create", "users.read", "users.update", "users.delete",
        "roles.assign",
        "shipments.create", "shipments.read", "shipments.update", "shipments.delete", "shipments.assign",
        "tracking.create", "tracking.read",
        "drivers.manage", "vehicles.manage", "warehouses.manage",
        "roles.create", "roles.read", "roles.update", "roles.delete"
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
builder.Services.AddScoped<ApiClient>();

await builder.Build().RunAsync();
