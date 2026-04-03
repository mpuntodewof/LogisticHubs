using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NiagaOne.E2E.Tests.Helpers;

public abstract class NiagaOneTestBase
{
    protected static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri("http://localhost:59980")
    };

    protected const string DefaultTenantId = "00000000-0000-0000-0000-000000000001";

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    protected static async Task<string> LoginAsync(string email, string password)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/login")
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
