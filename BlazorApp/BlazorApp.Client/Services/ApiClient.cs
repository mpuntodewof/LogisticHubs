using Blazored.LocalStorage;
using BlazorApp.Client.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BlazorApp.Client.Services
{
    public class ApiClient
    {
        private const string V1 = "api/v1";

        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;

        public ApiClient(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        private async Task AttachTokenAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("access_token");
            if (!string.IsNullOrEmpty(token))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<string?> ReadErrorMessageAsync(HttpResponseMessage response)
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

        private void AttachIdempotencyKey(string? idempotencyKey = null)
        {
            var key = idempotencyKey ?? Guid.NewGuid().ToString();
            _http.DefaultRequestHeaders.Remove("Idempotency-Key");
            _http.DefaultRequestHeaders.Add("Idempotency-Key", key);
        }

        private void ClearIdempotencyKey()
        {
            _http.DefaultRequestHeaders.Remove("Idempotency-Key");
        }

        // ── Auth ─────────────────────────────────────────────────────────────────

        public async Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{V1}/auth/login", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<LoginResponse>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return ApiResult<LoginResponse>.Ok(data!);
        }

        public async Task<ApiResult> RegisterAsync(RegisterRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{V1}/auth/register", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task LogoutAsync(string refreshToken)
        {
            await AttachTokenAsync();
            await _http.PostAsJsonAsync($"{V1}/auth/logout", new { refreshToken });
        }

        public async Task<ApiResult<LoginResponse>> RefreshTokenAsync(string refreshToken)
        {
            var response = await _http.PostAsJsonAsync($"{V1}/auth/refresh", new { refreshToken });
            if (!response.IsSuccessStatusCode)
                return ApiResult<LoginResponse>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return ApiResult<LoginResponse>.Ok(data!);
        }

        // ── Shipments ────────────────────────────────────────────────────────────

        public async Task<ApiResult<List<ShipmentDto>>> GetShipmentsAsync()
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<List<ShipmentDto>>($"{V1}/shipments") ?? new();
                return ApiResult<List<ShipmentDto>>.Ok(data);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<List<ShipmentDto>>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<ShipmentDto>> GetShipmentAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<ShipmentDto>($"{V1}/shipments/{id}");
                return ApiResult<ShipmentDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<ShipmentDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<ShipmentDto>> CreateShipmentAsync(CreateShipmentRequest request)
        {
            await AttachTokenAsync();
            AttachIdempotencyKey();
            var response = await _http.PostAsJsonAsync($"{V1}/shipments", request);
            ClearIdempotencyKey();
            if (!response.IsSuccessStatusCode)
                return ApiResult<ShipmentDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<ShipmentDto>();
            return ApiResult<ShipmentDto>.Ok(data!);
        }

        public async Task<ApiResult<List<TrackingEventDto>>> GetTrackingAsync(Guid shipmentId)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<List<TrackingEventDto>>($"{V1}/shipments/{shipmentId}/tracking") ?? new();
                return ApiResult<List<TrackingEventDto>>.Ok(data);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<List<TrackingEventDto>>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult> AssignShipmentAsync(Guid id, AssignShipmentRequest request)
        {
            await AttachTokenAsync();
            AttachIdempotencyKey();
            var response = await _http.PostAsJsonAsync($"{V1}/shipments/{id}/assign", request);
            ClearIdempotencyKey();
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult<TrackingEventDto>> AddTrackingEventAsync(Guid shipmentId, AddTrackingEventRequest request)
        {
            await AttachTokenAsync();
            AttachIdempotencyKey();
            var response = await _http.PostAsJsonAsync($"{V1}/shipments/{shipmentId}/tracking", request);
            ClearIdempotencyKey();
            if (!response.IsSuccessStatusCode)
                return ApiResult<TrackingEventDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<TrackingEventDto>();
            return ApiResult<TrackingEventDto>.Ok(data!);
        }

        // ── Drivers ──────────────────────────────────────────────────────────────

        public async Task<ApiResult<List<DriverDto>>> GetDriversAsync()
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<List<DriverDto>>($"{V1}/drivers") ?? new();
                return ApiResult<List<DriverDto>>.Ok(data);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<List<DriverDto>>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<DriverDto>> CreateDriverAsync(CreateDriverRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/drivers", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<DriverDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<DriverDto>();
            return ApiResult<DriverDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateDriverAsync(Guid id, UpdateDriverRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/drivers/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteDriverAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/drivers/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Vehicles ─────────────────────────────────────────────────────────────

        public async Task<ApiResult<List<VehicleDto>>> GetVehiclesAsync()
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<List<VehicleDto>>($"{V1}/vehicles") ?? new();
                return ApiResult<List<VehicleDto>>.Ok(data);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<List<VehicleDto>>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<VehicleDto>> CreateVehicleAsync(CreateVehicleRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/vehicles", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<VehicleDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<VehicleDto>();
            return ApiResult<VehicleDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateVehicleAsync(Guid id, UpdateVehicleRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/vehicles/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteVehicleAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/vehicles/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Warehouses ───────────────────────────────────────────────────────────

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

        // ── Users ────────────────────────────────────────────────────────────────

        public async Task<ApiResult<List<UserDto>>> GetUsersAsync()
        {
            await AttachTokenAsync();
            try
            {
                var paged = await _http.GetFromJsonAsync<PagedResult<UserDto>>($"{V1}/users?page=1&pageSize=200");
                var data = paged?.Items ?? new();
                return ApiResult<List<UserDto>>.Ok(data);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<List<UserDto>>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<UserDto>> GetUserAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<UserDto>($"{V1}/users/{id}");
                return ApiResult<UserDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<UserDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult> CreateUserAsync(CreateUserRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/auth/register", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> UpdateUserAsync(Guid id, UpdateUserRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/users/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteUserAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/users/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> AssignUserRoleAsync(Guid userId, AssignRoleRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/users/{userId}/roles", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> RevokeUserRoleAsync(Guid userId, Guid roleId)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/users/{userId}/roles/{roleId}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Roles ───────────────────────────────────────────────────────────────

        public async Task<ApiResult<List<RoleDto>>> GetRolesAsync()
        {
            await AttachTokenAsync();
            try
            {
                var paged = await _http.GetFromJsonAsync<PagedResult<RoleDto>>($"{V1}/roles?page=1&pageSize=200");
                var data = paged?.Items ?? new();
                return ApiResult<List<RoleDto>>.Ok(data);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<List<RoleDto>>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<RoleDto>> GetRoleAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<RoleDto>($"{V1}/roles/{id}");
                return ApiResult<RoleDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<RoleDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<RoleDto>> CreateRoleAsync(CreateRoleRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/roles", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<RoleDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<RoleDto>();
            return ApiResult<RoleDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateRoleAsync(Guid id, UpdateRoleRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/roles/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteRoleAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/roles/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> UpdateRolePermissionsAsync(Guid roleId, UpdateRolePermissionsRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/roles/{roleId}/permissions", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Permissions ─────────────────────────────────────────────────────────

        public async Task<ApiResult<List<PermissionDto>>> GetPermissionsAsync()
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<List<PermissionDto>>($"{V1}/permissions") ?? new();
                return ApiResult<List<PermissionDto>>.Ok(data);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<List<PermissionDto>>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        // ══════════════════════════════════════════════════════════════════════════
        // NEW MODULE API METHODS
        // ══════════════════════════════════════════════════════════════════════════

        // ── Categories ───────────────────────────────────────────────────────────

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

        public async Task<ApiResult<CategoryDto>> GetCategoryAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<CategoryDto>($"{V1}/categories/{id}");
                return ApiResult<CategoryDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<CategoryDto>.Fail((int?)ex.StatusCode ?? 500);
            }
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

        // ── Brands ───────────────────────────────────────────────────────────────

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

        public async Task<ApiResult<BrandDto>> GetBrandAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<BrandDto>($"{V1}/brands/{id}");
                return ApiResult<BrandDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<BrandDto>.Fail((int?)ex.StatusCode ?? 500);
            }
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

        // ── Products ─────────────────────────────────────────────────────────────

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

        public async Task<ApiResult<ProductDto>> GetProductAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<ProductDto>($"{V1}/products/{id}");
                return ApiResult<ProductDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<ProductDto>.Fail((int?)ex.StatusCode ?? 500);
            }
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

        // ── Units of Measure ─────────────────────────────────────────────────────

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

        // ── Warehouse Stock (Inventory) ──────────────────────────────────────────

        public async Task<ApiResult<PagedResult<object>>> GetWarehouseStockAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/warehouse-stock?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<object>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<object>>();
            return ApiResult<PagedResult<object>>.Ok(data!);
        }

        // ── Stock Movements ──────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<object>>> GetStockMovementsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/stock-movements?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<object>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<object>>();
            return ApiResult<PagedResult<object>>.Ok(data!);
        }

        // ── Sales Orders ─────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<SalesOrderDto>>> GetSalesOrdersAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/sales-orders?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<SalesOrderDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<SalesOrderDto>>();
            return ApiResult<PagedResult<SalesOrderDto>>.Ok(data!);
        }

        public async Task<ApiResult<SalesOrderDto>> GetSalesOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<SalesOrderDto>($"{V1}/sales-orders/{id}");
                return ApiResult<SalesOrderDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<SalesOrderDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<SalesOrderDto>> CreateSalesOrderAsync(CreateSalesOrderRequest request)
        {
            await AttachTokenAsync();
            AttachIdempotencyKey();
            var response = await _http.PostAsJsonAsync($"{V1}/sales-orders", request);
            ClearIdempotencyKey();
            if (!response.IsSuccessStatusCode)
                return ApiResult<SalesOrderDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<SalesOrderDto>();
            return ApiResult<SalesOrderDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateSalesOrderAsync(Guid id, CreateSalesOrderRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/sales-orders/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteSalesOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/sales-orders/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> ConfirmSalesOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            AttachIdempotencyKey();
            var response = await _http.PostAsync($"{V1}/sales-orders/{id}/confirm", null);
            ClearIdempotencyKey();
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> CancelSalesOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            AttachIdempotencyKey();
            var response = await _http.PostAsync($"{V1}/sales-orders/{id}/cancel", null);
            ClearIdempotencyKey();
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Customers ────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<CustomerDto>>> GetCustomersAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/customers?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<CustomerDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<CustomerDto>>();
            return ApiResult<PagedResult<CustomerDto>>.Ok(data!);
        }

        public async Task<ApiResult<CustomerDto>> GetCustomerAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<CustomerDto>($"{V1}/customers/{id}");
                return ApiResult<CustomerDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<CustomerDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/customers", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<CustomerDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<CustomerDto>();
            return ApiResult<CustomerDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateCustomerAsync(Guid id, CreateCustomerRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/customers/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteCustomerAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/customers/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Customer Groups ──────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<CustomerGroupDto>>> GetCustomerGroupsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/customer-groups?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<CustomerGroupDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<CustomerGroupDto>>();
            return ApiResult<PagedResult<CustomerGroupDto>>.Ok(data!);
        }

        public async Task<ApiResult<CustomerGroupDto>> CreateCustomerGroupAsync(CreateCustomerGroupRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/customer-groups", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<CustomerGroupDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<CustomerGroupDto>();
            return ApiResult<CustomerGroupDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateCustomerGroupAsync(Guid id, CreateCustomerGroupRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/customer-groups/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteCustomerGroupAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/customer-groups/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Purchase Orders ──────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<PurchaseOrderDto>>> GetPurchaseOrdersAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/purchase-orders?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<PurchaseOrderDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<PurchaseOrderDto>>();
            return ApiResult<PagedResult<PurchaseOrderDto>>.Ok(data!);
        }

        public async Task<ApiResult<PurchaseOrderDto>> GetPurchaseOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<PurchaseOrderDto>($"{V1}/purchase-orders/{id}");
                return ApiResult<PurchaseOrderDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<PurchaseOrderDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<PurchaseOrderDto>> CreatePurchaseOrderAsync(object request)
        {
            await AttachTokenAsync();
            AttachIdempotencyKey();
            var response = await _http.PostAsJsonAsync($"{V1}/purchase-orders", request);
            ClearIdempotencyKey();
            if (!response.IsSuccessStatusCode)
                return ApiResult<PurchaseOrderDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<PurchaseOrderDto>();
            return ApiResult<PurchaseOrderDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdatePurchaseOrderAsync(Guid id, object request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/purchase-orders/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeletePurchaseOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/purchase-orders/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Suppliers ────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<SupplierDto>>> GetSuppliersAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/suppliers?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<SupplierDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<SupplierDto>>();
            return ApiResult<PagedResult<SupplierDto>>.Ok(data!);
        }

        public async Task<ApiResult<SupplierDto>> GetSupplierAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<SupplierDto>($"{V1}/suppliers/{id}");
                return ApiResult<SupplierDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<SupplierDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<SupplierDto>> CreateSupplierAsync(CreateSupplierRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/suppliers", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<SupplierDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<SupplierDto>();
            return ApiResult<SupplierDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateSupplierAsync(Guid id, CreateSupplierRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/suppliers/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteSupplierAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/suppliers/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Goods Receipts ───────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<GoodsReceiptDto>>> GetGoodsReceiptsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/goods-receipts?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<GoodsReceiptDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<GoodsReceiptDto>>();
            return ApiResult<PagedResult<GoodsReceiptDto>>.Ok(data!);
        }

        public async Task<ApiResult<GoodsReceiptDto>> GetGoodsReceiptAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<GoodsReceiptDto>($"{V1}/goods-receipts/{id}");
                return ApiResult<GoodsReceiptDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<GoodsReceiptDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        // ── Chart of Accounts ────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<ChartOfAccountDto>>> GetChartOfAccountsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/chart-of-accounts?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<ChartOfAccountDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<ChartOfAccountDto>>();
            return ApiResult<PagedResult<ChartOfAccountDto>>.Ok(data!);
        }

        public async Task<ApiResult<ChartOfAccountDto>> GetChartOfAccountAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<ChartOfAccountDto>($"{V1}/chart-of-accounts/{id}");
                return ApiResult<ChartOfAccountDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<ChartOfAccountDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<ChartOfAccountDto>> CreateChartOfAccountAsync(CreateChartOfAccountRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/chart-of-accounts", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<ChartOfAccountDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<ChartOfAccountDto>();
            return ApiResult<ChartOfAccountDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateChartOfAccountAsync(Guid id, CreateChartOfAccountRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/chart-of-accounts/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteChartOfAccountAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/chart-of-accounts/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Journal Entries ──────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<JournalEntryDto>>> GetJournalEntriesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/journal-entries?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<JournalEntryDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<JournalEntryDto>>();
            return ApiResult<PagedResult<JournalEntryDto>>.Ok(data!);
        }

        public async Task<ApiResult<JournalEntryDto>> GetJournalEntryAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<JournalEntryDto>($"{V1}/journal-entries/{id}");
                return ApiResult<JournalEntryDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<JournalEntryDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<JournalEntryDto>> CreateJournalEntryAsync(CreateJournalEntryRequest request)
        {
            await AttachTokenAsync();
            AttachIdempotencyKey();
            var response = await _http.PostAsJsonAsync($"{V1}/journal-entries", request);
            ClearIdempotencyKey();
            if (!response.IsSuccessStatusCode)
                return ApiResult<JournalEntryDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<JournalEntryDto>();
            return ApiResult<JournalEntryDto>.Ok(data!);
        }

        public async Task<ApiResult> DeleteJournalEntryAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/journal-entries/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Invoices ─────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<InvoiceDto>>> GetInvoicesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/invoices?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<InvoiceDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<InvoiceDto>>();
            return ApiResult<PagedResult<InvoiceDto>>.Ok(data!);
        }

        public async Task<ApiResult<InvoiceDto>> GetInvoiceAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<InvoiceDto>($"{V1}/invoices/{id}");
                return ApiResult<InvoiceDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<InvoiceDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        // ── Tax Rates ────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<TaxRateDto>>> GetTaxRatesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/tax-rates?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<TaxRateDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<TaxRateDto>>();
            return ApiResult<PagedResult<TaxRateDto>>.Ok(data!);
        }

        public async Task<ApiResult<TaxRateDto>> CreateTaxRateAsync(CreateTaxRateRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/tax-rates", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<TaxRateDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<TaxRateDto>();
            return ApiResult<TaxRateDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateTaxRateAsync(Guid id, CreateTaxRateRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/tax-rates/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteTaxRateAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/tax-rates/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Payment Terms ────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<PaymentTermDto>>> GetPaymentTermsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/payment-terms?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<PaymentTermDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<PaymentTermDto>>();
            return ApiResult<PagedResult<PaymentTermDto>>.Ok(data!);
        }

        public async Task<ApiResult<PaymentTermDto>> CreatePaymentTermAsync(CreatePaymentTermRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/payment-terms", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<PaymentTermDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<PaymentTermDto>();
            return ApiResult<PaymentTermDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdatePaymentTermAsync(Guid id, CreatePaymentTermRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/payment-terms/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeletePaymentTermAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/payment-terms/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Promotions ───────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<PromotionDto>>> GetPromotionsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/promotions?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<PromotionDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<PromotionDto>>();
            return ApiResult<PagedResult<PromotionDto>>.Ok(data!);
        }

        public async Task<ApiResult<PromotionDto>> GetPromotionAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<PromotionDto>($"{V1}/promotions/{id}");
                return ApiResult<PromotionDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<PromotionDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<PromotionDto>> CreatePromotionAsync(CreatePromotionRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/promotions", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<PromotionDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<PromotionDto>();
            return ApiResult<PromotionDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdatePromotionAsync(Guid id, CreatePromotionRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/promotions/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeletePromotionAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/promotions/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Coupons ──────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<CouponCodeDto>>> GetCouponsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/coupons?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<CouponCodeDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<CouponCodeDto>>();
            return ApiResult<PagedResult<CouponCodeDto>>.Ok(data!);
        }

        public async Task<ApiResult<CouponCodeDto>> CreateCouponAsync(CreateCouponCodeRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/coupons", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<CouponCodeDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<CouponCodeDto>();
            return ApiResult<CouponCodeDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateCouponAsync(Guid id, CreateCouponCodeRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/coupons/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteCouponAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/coupons/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Loyalty Programs ─────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<LoyaltyProgramDto>>> GetLoyaltyProgramsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/loyalty-programs?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<LoyaltyProgramDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<LoyaltyProgramDto>>();
            return ApiResult<PagedResult<LoyaltyProgramDto>>.Ok(data!);
        }

        public async Task<ApiResult<LoyaltyProgramDto>> GetLoyaltyProgramAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<LoyaltyProgramDto>($"{V1}/loyalty-programs/{id}");
                return ApiResult<LoyaltyProgramDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<LoyaltyProgramDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        // ── Loyalty Memberships ──────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<LoyaltyMembershipDto>>> GetLoyaltyMembershipsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/loyalty-memberships?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<LoyaltyMembershipDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<LoyaltyMembershipDto>>();
            return ApiResult<PagedResult<LoyaltyMembershipDto>>.Ok(data!);
        }

        // ── Departments ──────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<DepartmentDto>>> GetDepartmentsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/departments?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<DepartmentDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<DepartmentDto>>();
            return ApiResult<PagedResult<DepartmentDto>>.Ok(data!);
        }

        public async Task<ApiResult<DepartmentDto>> GetDepartmentAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<DepartmentDto>($"{V1}/departments/{id}");
                return ApiResult<DepartmentDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<DepartmentDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<DepartmentDto>> CreateDepartmentAsync(CreateDepartmentRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/departments", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<DepartmentDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<DepartmentDto>();
            return ApiResult<DepartmentDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateDepartmentAsync(Guid id, CreateDepartmentRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/departments/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteDepartmentAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/departments/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Employees ────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<EmployeeDto>>> GetEmployeesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/employees?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<EmployeeDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<EmployeeDto>>();
            return ApiResult<PagedResult<EmployeeDto>>.Ok(data!);
        }

        public async Task<ApiResult<EmployeeDto>> GetEmployeeAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<EmployeeDto>($"{V1}/employees/{id}");
                return ApiResult<EmployeeDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<EmployeeDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/employees", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<EmployeeDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<EmployeeDto>();
            return ApiResult<EmployeeDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateEmployeeAsync(Guid id, CreateEmployeeRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/employees/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteEmployeeAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/employees/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Attendance ───────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<AttendanceDto>>> GetAttendanceAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/attendance?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<AttendanceDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<AttendanceDto>>();
            return ApiResult<PagedResult<AttendanceDto>>.Ok(data!);
        }

        public async Task<ApiResult> ClockInAsync()
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync($"{V1}/attendance/clock-in", null);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> ClockOutAsync()
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync($"{V1}/attendance/clock-out", null);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Leave Requests ───────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<LeaveRequestDto>>> GetLeaveRequestsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/leave-requests?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<LeaveRequestDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<LeaveRequestDto>>();
            return ApiResult<PagedResult<LeaveRequestDto>>.Ok(data!);
        }

        public async Task<ApiResult<LeaveRequestDto>> CreateLeaveRequestAsync(CreateLeaveRequestRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/leave-requests", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<LeaveRequestDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<LeaveRequestDto>();
            return ApiResult<LeaveRequestDto>.Ok(data!);
        }

        public async Task<ApiResult> ApproveLeaveRequestAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync($"{V1}/leave-requests/{id}/approve", null);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> RejectLeaveRequestAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync($"{V1}/leave-requests/{id}/reject", null);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Branches ─────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<BranchDto>>> GetBranchesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/branches?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<BranchDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<BranchDto>>();
            return ApiResult<PagedResult<BranchDto>>.Ok(data!);
        }

        public async Task<ApiResult<BranchDto>> GetBranchAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<BranchDto>($"{V1}/branches/{id}");
                return ApiResult<BranchDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<BranchDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<BranchDto>> CreateBranchAsync(CreateBranchRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/branches", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<BranchDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<BranchDto>();
            return ApiResult<BranchDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateBranchAsync(Guid id, CreateBranchRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/branches/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteBranchAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/branches/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Delivery Zones ───────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<DeliveryZoneDto>>> GetDeliveryZonesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/delivery-zones?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<DeliveryZoneDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<DeliveryZoneDto>>();
            return ApiResult<PagedResult<DeliveryZoneDto>>.Ok(data!);
        }

        public async Task<ApiResult<DeliveryZoneDto>> GetDeliveryZoneAsync(Guid id)
        {
            await AttachTokenAsync();
            try
            {
                var data = await _http.GetFromJsonAsync<DeliveryZoneDto>($"{V1}/delivery-zones/{id}");
                return ApiResult<DeliveryZoneDto>.Ok(data!);
            }
            catch (HttpRequestException ex)
            {
                return ApiResult<DeliveryZoneDto>.Fail((int?)ex.StatusCode ?? 500);
            }
        }

        public async Task<ApiResult<DeliveryZoneDto>> CreateDeliveryZoneAsync(CreateDeliveryZoneRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/delivery-zones", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<DeliveryZoneDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<DeliveryZoneDto>();
            return ApiResult<DeliveryZoneDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateDeliveryZoneAsync(Guid id, CreateDeliveryZoneRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/delivery-zones/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteDeliveryZoneAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/delivery-zones/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Notifications ────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<NotificationDto>>> GetMyNotificationsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/notifications/my?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<NotificationDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<NotificationDto>>();
            return ApiResult<PagedResult<NotificationDto>>.Ok(data!);
        }

        public async Task<ApiResult<int>> GetUnreadCountAsync()
        {
            await AttachTokenAsync();
            var resp = await _http.GetAsync($"{V1}/notifications/unread-count");
            if (!resp.IsSuccessStatusCode)
                return ApiResult<int>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<int>();
            return ApiResult<int>.Ok(data);
        }

        public async Task<ApiResult> MarkNotificationReadAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync($"{V1}/notifications/{id}/read", null);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Tenant Settings ──────────────────────────────────────────────────────

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

        // ── Reports ──────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<ReportDefinitionDto>>> GetReportsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/reports?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<ReportDefinitionDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<ReportDefinitionDto>>();
            return ApiResult<PagedResult<ReportDefinitionDto>>.Ok(data!);
        }

        // ── Audit Logs ───────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<AuditLogDto>>> GetAuditLogsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/audit-logs?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<AuditLogDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<AuditLogDto>>();
            return ApiResult<PagedResult<AuditLogDto>>.Ok(data!);
        }

        // ── API Keys ─────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<ApiKeyDto>>> GetApiKeysAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/api-keys?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<ApiKeyDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<ApiKeyDto>>();
            return ApiResult<PagedResult<ApiKeyDto>>.Ok(data!);
        }

        // ── Webhooks ─────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<WebhookSubscriptionDto>>> GetWebhooksAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/webhooks?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<WebhookSubscriptionDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<WebhookSubscriptionDto>>();
            return ApiResult<PagedResult<WebhookSubscriptionDto>>.Ok(data!);
        }

        // ── Banners ──────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<BannerDto>>> GetBannersAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/banners?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<BannerDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<BannerDto>>();
            return ApiResult<PagedResult<BannerDto>>.Ok(data!);
        }

        public async Task<ApiResult<BannerDto>> CreateBannerAsync(CreateBannerRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/banners", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<BannerDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<BannerDto>();
            return ApiResult<BannerDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateBannerAsync(Guid id, CreateBannerRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/banners/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteBannerAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/banners/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Pages ────────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<PageDto>>> GetPagesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/pages?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<PageDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<PageDto>>();
            return ApiResult<PagedResult<PageDto>>.Ok(data!);
        }

        public async Task<ApiResult<PageDto>> CreatePageAsync(CreatePageRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"{V1}/pages", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<PageDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<PageDto>();
            return ApiResult<PageDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdatePageAsync(Guid id, CreatePageRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"{V1}/pages/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeletePageAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"{V1}/pages/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Storefront Config ────────────────────────────────────────────────────

        public async Task<ApiResult<StorefrontConfigDto>> GetStorefrontConfigAsync()
        {
            await AttachTokenAsync();
            var resp = await _http.GetAsync($"{V1}/storefront-config");
            if (!resp.IsSuccessStatusCode)
                return ApiResult<StorefrontConfigDto>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<StorefrontConfigDto>();
            return ApiResult<StorefrontConfigDto>.Ok(data!);
        }

        // ── Import ───────────────────────────────────────────────────────────────

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

        public async Task<ApiResult<PagedResult<ImportBatchDto>>> GetImportBatchesAsync(int page = 1, int pageSize = 20)
        {
            await AttachTokenAsync();
            var resp = await _http.GetAsync($"{V1}/import/batches?page={page}&pageSize={pageSize}");
            if (!resp.IsSuccessStatusCode)
                return ApiResult<PagedResult<ImportBatchDto>>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<PagedResult<ImportBatchDto>>();
            return ApiResult<PagedResult<ImportBatchDto>>.Ok(data!);
        }

        // ── Reports & Dashboard ──────────────────────────────────────────────────

        public async Task<ApiResult<DashboardSummary>> GetDashboardSummaryAsync()
        {
            await AttachTokenAsync();
            var resp = await _http.GetAsync($"{V1}/reports/dashboard");
            if (!resp.IsSuccessStatusCode)
                return ApiResult<DashboardSummary>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<DashboardSummary>();
            return ApiResult<DashboardSummary>.Ok(data!);
        }

        public async Task<ApiResult<ProfitAndLossReport>> GetProfitAndLossAsync(DateTime? from = null, DateTime? to = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/reports/profit-and-loss";
            var queryParams = new List<string>();
            if (from.HasValue) queryParams.Add($"from={from.Value:yyyy-MM-dd}");
            if (to.HasValue) queryParams.Add($"to={to.Value:yyyy-MM-dd}");
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<ProfitAndLossReport>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<ProfitAndLossReport>();
            return ApiResult<ProfitAndLossReport>.Ok(data!);
        }

        public async Task<ApiResult<NotificationSummary>> GetNotificationsAsync()
        {
            await AttachTokenAsync();
            var resp = await _http.GetAsync($"{V1}/notifications");
            if (!resp.IsSuccessStatusCode)
                return ApiResult<NotificationSummary>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<NotificationSummary>();
            return ApiResult<NotificationSummary>.Ok(data!);
        }

        // ── Export ───────────────────────────────────────────────────────────────

        public async Task<ApiResult<byte[]>> ExportAsync(string entityType, string? status = null)
        {
            await AttachTokenAsync();
            var url = $"{V1}/export/{entityType}";
            if (!string.IsNullOrEmpty(status)) url += $"?status={Uri.EscapeDataString(status)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
                return ApiResult<byte[]>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var bytes = await resp.Content.ReadAsByteArrayAsync();
            return ApiResult<byte[]>.Ok(bytes);
        }
    }
}
