using Blazored.LocalStorage;
using BlazorApp.Client.Models;
using System.Net.Http.Json;

namespace BlazorApp.Client.Services.ApiClients
{
    public class ImportClient : BaseApiClient
    {
        public ImportClient(HttpClient http, ILocalStorageService localStorage) : base(http, localStorage) { }

        public async Task<ApiResult<PagedResult<SalesChannelDto>>> GetSalesChannelsAsync(int page = 1, int pageSize = 20)
        {
            await AttachTokenAsync();
            var resp = await _http.GetAsync($"{V1}/import/channels?page={page}&pageSize={pageSize}");
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<SalesChannelDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<SalesChannelDto>>();
            return ApiResult<PagedResult<SalesChannelDto>>.Ok(data!);
        }

        public async Task<ApiResult<SalesChannelDto>> CreateSalesChannelAsync(CreateSalesChannelRequest request)
        {
            await AttachTokenAsync();
            AttachIdempotencyKey();
            var resp = await _http.PostAsJsonAsync($"{V1}/import/channels", request);
            ClearIdempotencyKey();
            if (!resp.IsSuccessStatusCode)
                return ApiResult<SalesChannelDto>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<SalesChannelDto>();
            return ApiResult<SalesChannelDto>.Ok(data!);
        }

        public async Task<ApiResult<List<string>>> PreviewCsvHeadersAsync(Stream fileStream, string fileName)
        {
            await AttachTokenAsync();
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(fileStream), "file", fileName);
            var resp = await _http.PostAsync($"{V1}/import/csv/preview", content);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<List<string>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<List<string>>();
            return ApiResult<List<string>>.Ok(data!);
        }

        public async Task<ApiResult<ImportSummaryDto>> ProcessCsvImportAsync(
            Stream fileStream, string fileName, Guid salesChannelId, Guid warehouseId,
            string orderNumberCol, string skuCol, string quantityCol, string unitPriceCol,
            string? totalPriceCol = null, string? productNameCol = null, string? orderDateCol = null, string? platformFeeCol = null)
        {
            await AttachTokenAsync();
            AttachIdempotencyKey();
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(fileStream), "file", fileName);
            content.Add(new StringContent(salesChannelId.ToString()), "salesChannelId");
            content.Add(new StringContent(warehouseId.ToString()), "warehouseId");
            content.Add(new StringContent(orderNumberCol), "orderNumberColumn");
            content.Add(new StringContent(skuCol), "skuColumn");
            content.Add(new StringContent(quantityCol), "quantityColumn");
            content.Add(new StringContent(unitPriceCol), "unitPriceColumn");
            if (totalPriceCol != null) content.Add(new StringContent(totalPriceCol), "totalPriceColumn");
            if (productNameCol != null) content.Add(new StringContent(productNameCol), "productNameColumn");
            if (orderDateCol != null) content.Add(new StringContent(orderDateCol), "orderDateColumn");
            if (platformFeeCol != null) content.Add(new StringContent(platformFeeCol), "platformFeeColumn");
            var resp = await _http.PostAsync($"{V1}/import/csv/process", content);
            ClearIdempotencyKey();
            if (!resp.IsSuccessStatusCode)
                return ApiResult<ImportSummaryDto>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<ImportSummaryDto>();
            return ApiResult<ImportSummaryDto>.Ok(data!);
        }

        public async Task<ApiResult<InitialStockResultDto>> ProcessInitialStockImportAsync(
            Stream fileStream, string fileName, Guid warehouseId, string skuCol, string quantityCol)
        {
            await AttachTokenAsync();
            AttachIdempotencyKey();
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(fileStream), "file", fileName);
            content.Add(new StringContent(warehouseId.ToString()), "warehouseId");
            content.Add(new StringContent(skuCol), "skuColumn");
            content.Add(new StringContent(quantityCol), "quantityColumn");
            var resp = await _http.PostAsync($"{V1}/import/csv/initial-stock", content);
            ClearIdempotencyKey();
            if (!resp.IsSuccessStatusCode)
                return ApiResult<InitialStockResultDto>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<InitialStockResultDto>();
            return ApiResult<InitialStockResultDto>.Ok(data!);
        }

        public async Task<ApiResult<PagedResult<ImportBatchDto>>> GetImportBatchesAsync(int page = 1, int pageSize = 20)
        {
            await AttachTokenAsync();
            var resp = await _http.GetAsync($"{V1}/import/batches?page={page}&pageSize={pageSize}");
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<ImportBatchDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<ImportBatchDto>>();
            return ApiResult<PagedResult<ImportBatchDto>>.Ok(data!);
        }
    }
}
