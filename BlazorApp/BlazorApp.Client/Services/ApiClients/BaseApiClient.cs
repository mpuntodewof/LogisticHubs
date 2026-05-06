using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace BlazorApp.Client.Services.ApiClients
{
    public abstract class BaseApiClient
    {
        protected const string V1 = "api/v1";

        protected readonly HttpClient _http;
        protected readonly ILocalStorageService _localStorage;

        protected BaseApiClient(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        protected async Task AttachTokenAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("access_token");
            if (!string.IsNullOrEmpty(token))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected async Task<string?> ReadErrorMessageAsync(HttpResponseMessage response)
        {
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content)) return null;
                var json = System.Text.Json.JsonDocument.Parse(content);
                if (json.RootElement.TryGetProperty("error", out var errorProp))
                    return errorProp.GetString();
                return content;
            }
            catch { return null; }
        }

        protected void AttachIdempotencyKey(string? idempotencyKey = null)
        {
            var key = idempotencyKey ?? Guid.NewGuid().ToString();
            _http.DefaultRequestHeaders.Remove("Idempotency-Key");
            _http.DefaultRequestHeaders.Add("Idempotency-Key", key);
        }

        protected void ClearIdempotencyKey()
        {
            _http.DefaultRequestHeaders.Remove("Idempotency-Key");
        }
    }
}
