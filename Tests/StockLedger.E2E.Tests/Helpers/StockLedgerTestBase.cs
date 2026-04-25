using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StockLedger.E2E.Tests.Helpers;

public abstract class StockLedgerTestBase
{
    protected static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri("http://localhost:5164")
    };

    protected const string V1 = "/api/v1";
    protected const string DefaultTenantId = "00000000-0000-0000-0000-000000000001";

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    // Token cache — the /auth/login endpoint is rate-limited to 5 req/min per IP.
    // xUnit creates a new test-class instance per [Fact], so caching by credentials
    // keeps the whole suite under the limit while still exercising the full login path once.
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Lazy<Task<string>>> _tokenCache = new();

    protected static Task<string> LoginAsync(string email, string password)
    {
        var key = $"{email}|{password}";
        var lazy = _tokenCache.GetOrAdd(key, _ => new Lazy<Task<string>>(() => LoginDirectAsync(email, password)));
        return lazy.Value;
    }

    private static async Task<string> LoginDirectAsync(string email, string password)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{V1}/auth/login")
        {
            Content = JsonContent.Create(new { email, password }, options: JsonOptions)
        };
        request.Headers.Add("X-Tenant-Id", DefaultTenantId);

        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        return result.GetProperty("accessToken").GetString()!;
    }

    protected static async Task<HttpResponseMessage> AuthGet(string token, string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Tenant-Id", DefaultTenantId);
        return await Client.SendAsync(request);
    }

    protected static async Task<HttpResponseMessage> AuthPost(string token, string url, object? body = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Tenant-Id", DefaultTenantId);

        if (body is not null)
        {
            var json = JsonSerializer.Serialize(body, JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return await Client.SendAsync(request);
    }

    protected static async Task<HttpResponseMessage> AuthPut(string token, string url, object body)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Tenant-Id", DefaultTenantId);

        var json = JsonSerializer.Serialize(body, JsonOptions);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        return await Client.SendAsync(request);
    }

    protected static async Task<HttpResponseMessage> AuthDelete(string token, string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Tenant-Id", DefaultTenantId);
        return await Client.SendAsync(request);
    }

    protected static async Task<T> ReadAs<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, JsonOptions)!;
    }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
