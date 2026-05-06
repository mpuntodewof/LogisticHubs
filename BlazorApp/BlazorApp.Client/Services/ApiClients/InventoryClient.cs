using Blazored.LocalStorage;
using BlazorApp.Client.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorApp.Client.Services.ApiClients
{
    public class InventoryClient : BaseApiClient
    {
        public InventoryClient(HttpClient http, ILocalStorageService localStorage) : base(http, localStorage) { }

        #region Warehouses

        public async Task<ApiResult<List<WarehouseDto>>> GetWarehousesAsync()
        {
            await AttachTokenAsync();
            try
            {
                var paged = await _http.GetFromJsonAsync<PagedResult<WarehouseDto>>($"{V1}/warehouses?page=1&pageSize=200");
                var data = paged?.Items ?? new();
                return ApiResult<List<WarehouseDto>>.Ok(data);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<List<WarehouseDto>>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<WarehouseDto>> CreateWarehouseAsync(CreateWarehouseRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/warehouses", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<WarehouseDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<WarehouseDto>();
            return ApiResult<WarehouseDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateWarehouseAsync(Guid id, UpdateWarehouseRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/warehouses/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteWarehouseAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/warehouses/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        #endregion

        #region Warehouse Stock

        public async Task<ApiResult<PagedResult<JsonElement>>> GetWarehouseStockAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/warehouse-stock?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<JsonElement>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<JsonElement>>();
            return ApiResult<PagedResult<JsonElement>>.Ok(data!);
        }

        #endregion

        #region Stock Movements

        public async Task<ApiResult<PagedResult<JsonElement>>> GetStockMovementsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/stock-movements?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<JsonElement>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<JsonElement>>();
            return ApiResult<PagedResult<JsonElement>>.Ok(data!);
        }

        #endregion
    }
}
