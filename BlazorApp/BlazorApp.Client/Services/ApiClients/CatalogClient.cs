using Blazored.LocalStorage;
using BlazorApp.Client.Models;
using System.Net.Http.Json;

namespace BlazorApp.Client.Services.ApiClients
{
    public class CatalogClient : BaseApiClient
    {
        public CatalogClient(HttpClient http, ILocalStorageService localStorage) : base(http, localStorage) { }

        #region Categories

        public async Task<ApiResult<PagedResult<CategoryDto>>> GetCategoriesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/categories?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<CategoryDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<CategoryDto>>();
            return ApiResult<PagedResult<CategoryDto>>.Ok(data!);
        }

        public async Task<ApiResult<CategoryDto>> CreateCategoryAsync(CreateCategoryRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/categories", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<CategoryDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<CategoryDto>();
            return ApiResult<CategoryDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateCategoryAsync(Guid id, CreateCategoryRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/categories/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteCategoryAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/categories/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        #endregion

        #region Brands

        public async Task<ApiResult<PagedResult<BrandDto>>> GetBrandsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/brands?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<BrandDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<BrandDto>>();
            return ApiResult<PagedResult<BrandDto>>.Ok(data!);
        }

        public async Task<ApiResult<BrandDto>> CreateBrandAsync(CreateBrandRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/brands", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<BrandDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<BrandDto>();
            return ApiResult<BrandDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateBrandAsync(Guid id, CreateBrandRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/brands/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteBrandAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/brands/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        #endregion

        #region Products

        public async Task<ApiResult<PagedResult<ProductDto>>> GetProductsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/products?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<ProductDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
            return ApiResult<PagedResult<ProductDto>>.Ok(data!);
        }

        public async Task<ApiResult<ProductDto>> CreateProductAsync(CreateProductRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/products", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<ProductDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<ProductDto>();
            return ApiResult<ProductDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateProductAsync(Guid id, CreateProductRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/products/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteProductAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/products/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        #endregion

        #region Units of Measure

        public async Task<ApiResult<PagedResult<UnitOfMeasureDto>>> GetUnitsOfMeasureAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/units-of-measure?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<UnitOfMeasureDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<UnitOfMeasureDto>>();
            return ApiResult<PagedResult<UnitOfMeasureDto>>.Ok(data!);
        }

        public async Task<ApiResult<UnitOfMeasureDto>> CreateUnitOfMeasureAsync(CreateUnitOfMeasureRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/units-of-measure", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<UnitOfMeasureDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<UnitOfMeasureDto>();
            return ApiResult<UnitOfMeasureDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateUnitOfMeasureAsync(Guid id, CreateUnitOfMeasureRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/units-of-measure/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteUnitOfMeasureAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/units-of-measure/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        #endregion
    }
}
