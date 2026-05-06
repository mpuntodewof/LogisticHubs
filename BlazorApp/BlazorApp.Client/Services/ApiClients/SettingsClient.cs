using Blazored.LocalStorage;
using BlazorApp.Client.Models;
using System.Net.Http.Json;

namespace BlazorApp.Client.Services.ApiClients
{
    public class SettingsClient : BaseApiClient
    {
        public SettingsClient(HttpClient http, ILocalStorageService localStorage) : base(http, localStorage) { }

        public async Task<ApiResult<PagedResult<TenantSettingDto>>> GetTenantSettingsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/tenant-settings?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<TenantSettingDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<TenantSettingDto>>();
            return ApiResult<PagedResult<TenantSettingDto>>.Ok(data!);
        }

        public async Task<ApiResult> UpdateTenantSettingAsync(Guid id, object request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/tenant-settings/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }
    }
}
