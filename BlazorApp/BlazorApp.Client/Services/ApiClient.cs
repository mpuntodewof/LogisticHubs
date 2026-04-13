using Blazored.LocalStorage;
using BlazorApp.Client.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BlazorApp.Client.Services
{
    public class ApiClient
    {
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
            var response = await _http.PostAsJsonAsync("api/auth/login", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<LoginResponse>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return ApiResult<LoginResponse>.Ok(data!);
        }

        public async Task<ApiResult> RegisterAsync(RegisterRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task LogoutAsync(string refreshToken)
        {
            await AttachTokenAsync();
            await _http.PostAsJsonAsync("api/auth/logout", new { refreshToken });
        }

        public async Task<ApiResult<LoginResponse>> RefreshTokenAsync(string refreshToken)
        {
            var response = await _http.PostAsJsonAsync("api/auth/refresh", new { refreshToken });
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
                var data = await _http.GetFromJsonAsync<List<ShipmentDto>>("api/shipments") ?? new();
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
                var data = await _http.GetFromJsonAsync<ShipmentDto>($"api/shipments/{id}");
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
            var response = await _http.PostAsJsonAsync("api/shipments", request);
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
                var data = await _http.GetFromJsonAsync<List<TrackingEventDto>>($"api/shipments/{shipmentId}/tracking") ?? new();
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
            var response = await _http.PostAsJsonAsync($"api/shipments/{id}/assign", request);
            ClearIdempotencyKey();
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult<TrackingEventDto>> AddTrackingEventAsync(Guid shipmentId, AddTrackingEventRequest request)
        {
            await AttachTokenAsync();
            AttachIdempotencyKey();
            var response = await _http.PostAsJsonAsync($"api/shipments/{shipmentId}/tracking", request);
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
                var data = await _http.GetFromJsonAsync<List<DriverDto>>("api/drivers") ?? new();
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
            var response = await _http.PostAsJsonAsync("api/drivers", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<DriverDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<DriverDto>();
            return ApiResult<DriverDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateDriverAsync(Guid id, UpdateDriverRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/drivers/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteDriverAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/drivers/{id}");
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
                var data = await _http.GetFromJsonAsync<List<VehicleDto>>("api/vehicles") ?? new();
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
            var response = await _http.PostAsJsonAsync("api/vehicles", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<VehicleDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<VehicleDto>();
            return ApiResult<VehicleDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateVehicleAsync(Guid id, UpdateVehicleRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/vehicles/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteVehicleAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/vehicles/{id}");
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
                var paged = await _http.GetFromJsonAsync<PagedResult<WarehouseDto>>("api/warehouses?page=1&pageSize=200");
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
            var response = await _http.PostAsJsonAsync("api/warehouses", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<WarehouseDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<WarehouseDto>();
            return ApiResult<WarehouseDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateWarehouseAsync(Guid id, UpdateWarehouseRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/warehouses/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteWarehouseAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/warehouses/{id}");
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
                var paged = await _http.GetFromJsonAsync<PagedResult<UserDto>>("api/users?page=1&pageSize=200");
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
                var data = await _http.GetFromJsonAsync<UserDto>($"api/users/{id}");
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
            var response = await _http.PostAsJsonAsync("api/auth/register", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> UpdateUserAsync(Guid id, UpdateUserRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/users/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteUserAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/users/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> AssignUserRoleAsync(Guid userId, AssignRoleRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"api/users/{userId}/roles", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> RevokeUserRoleAsync(Guid userId, Guid roleId)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/users/{userId}/roles/{roleId}");
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
                var paged = await _http.GetFromJsonAsync<PagedResult<RoleDto>>("api/roles?page=1&pageSize=200");
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
                var data = await _http.GetFromJsonAsync<RoleDto>($"api/roles/{id}");
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
            var response = await _http.PostAsJsonAsync("api/roles", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<RoleDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<RoleDto>();
            return ApiResult<RoleDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateRoleAsync(Guid id, UpdateRoleRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/roles/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteRoleAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/roles/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> UpdateRolePermissionsAsync(Guid roleId, UpdateRolePermissionsRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/roles/{roleId}/permissions", request);
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
                var data = await _http.GetFromJsonAsync<List<PermissionDto>>("api/permissions") ?? new();
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
            var url = $"api/categories?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<CategoryDto>($"api/categories/{id}");
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
            var response = await _http.PostAsJsonAsync("api/categories", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<CategoryDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<CategoryDto>();
            return ApiResult<CategoryDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateCategoryAsync(Guid id, CreateCategoryRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/categories/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteCategoryAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/categories/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Brands ───────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<BrandDto>>> GetBrandsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/brands?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<BrandDto>($"api/brands/{id}");
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
            var response = await _http.PostAsJsonAsync("api/brands", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<BrandDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<BrandDto>();
            return ApiResult<BrandDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateBrandAsync(Guid id, CreateBrandRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/brands/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteBrandAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/brands/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Products ─────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<ProductDto>>> GetProductsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/products?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<ProductDto>($"api/products/{id}");
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
            var response = await _http.PostAsJsonAsync("api/products", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<ProductDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<ProductDto>();
            return ApiResult<ProductDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateProductAsync(Guid id, CreateProductRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/products/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteProductAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/products/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Units of Measure ─────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<UnitOfMeasureDto>>> GetUnitsOfMeasureAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/units-of-measure?page={page}&pageSize={pageSize}";
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
            var response = await _http.PostAsJsonAsync("api/units-of-measure", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<UnitOfMeasureDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<UnitOfMeasureDto>();
            return ApiResult<UnitOfMeasureDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateUnitOfMeasureAsync(Guid id, CreateUnitOfMeasureRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/units-of-measure/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteUnitOfMeasureAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/units-of-measure/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Warehouse Stock (Inventory) ──────────────────────────────────────────

        public async Task<ApiResult<PagedResult<object>>> GetWarehouseStockAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/warehouse-stock?page={page}&pageSize={pageSize}";
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
            var url = $"api/stock-movements?page={page}&pageSize={pageSize}";
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
            var url = $"api/sales-orders?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<SalesOrderDto>($"api/sales-orders/{id}");
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
            var response = await _http.PostAsJsonAsync("api/sales-orders", request);
            ClearIdempotencyKey();
            if (!response.IsSuccessStatusCode)
                return ApiResult<SalesOrderDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<SalesOrderDto>();
            return ApiResult<SalesOrderDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateSalesOrderAsync(Guid id, CreateSalesOrderRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/sales-orders/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteSalesOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/sales-orders/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> ConfirmSalesOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            AttachIdempotencyKey();
            var response = await _http.PostAsync($"api/sales-orders/{id}/confirm", null);
            ClearIdempotencyKey();
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> CancelSalesOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            AttachIdempotencyKey();
            var response = await _http.PostAsync($"api/sales-orders/{id}/cancel", null);
            ClearIdempotencyKey();
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Customers ────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<CustomerDto>>> GetCustomersAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/customers?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<CustomerDto>($"api/customers/{id}");
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
            var response = await _http.PostAsJsonAsync("api/customers", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<CustomerDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<CustomerDto>();
            return ApiResult<CustomerDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateCustomerAsync(Guid id, CreateCustomerRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/customers/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteCustomerAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/customers/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Customer Groups ──────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<CustomerGroupDto>>> GetCustomerGroupsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/customer-groups?page={page}&pageSize={pageSize}";
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
            var response = await _http.PostAsJsonAsync("api/customer-groups", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<CustomerGroupDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<CustomerGroupDto>();
            return ApiResult<CustomerGroupDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateCustomerGroupAsync(Guid id, CreateCustomerGroupRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/customer-groups/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteCustomerGroupAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/customer-groups/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Purchase Orders ──────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<PurchaseOrderDto>>> GetPurchaseOrdersAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/purchase-orders?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<PurchaseOrderDto>($"api/purchase-orders/{id}");
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
            var response = await _http.PostAsJsonAsync("api/purchase-orders", request);
            ClearIdempotencyKey();
            if (!response.IsSuccessStatusCode)
                return ApiResult<PurchaseOrderDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<PurchaseOrderDto>();
            return ApiResult<PurchaseOrderDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdatePurchaseOrderAsync(Guid id, object request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/purchase-orders/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeletePurchaseOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/purchase-orders/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Suppliers ────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<SupplierDto>>> GetSuppliersAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/suppliers?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<SupplierDto>($"api/suppliers/{id}");
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
            var response = await _http.PostAsJsonAsync("api/suppliers", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<SupplierDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<SupplierDto>();
            return ApiResult<SupplierDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateSupplierAsync(Guid id, CreateSupplierRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/suppliers/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteSupplierAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/suppliers/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Goods Receipts ───────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<GoodsReceiptDto>>> GetGoodsReceiptsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/goods-receipts?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<GoodsReceiptDto>($"api/goods-receipts/{id}");
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
            var url = $"api/chart-of-accounts?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<ChartOfAccountDto>($"api/chart-of-accounts/{id}");
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
            var response = await _http.PostAsJsonAsync("api/chart-of-accounts", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<ChartOfAccountDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<ChartOfAccountDto>();
            return ApiResult<ChartOfAccountDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateChartOfAccountAsync(Guid id, CreateChartOfAccountRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/chart-of-accounts/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteChartOfAccountAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/chart-of-accounts/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Journal Entries ──────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<JournalEntryDto>>> GetJournalEntriesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/journal-entries?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<JournalEntryDto>($"api/journal-entries/{id}");
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
            var response = await _http.PostAsJsonAsync("api/journal-entries", request);
            ClearIdempotencyKey();
            if (!response.IsSuccessStatusCode)
                return ApiResult<JournalEntryDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<JournalEntryDto>();
            return ApiResult<JournalEntryDto>.Ok(data!);
        }

        public async Task<ApiResult> DeleteJournalEntryAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/journal-entries/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Invoices ─────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<InvoiceDto>>> GetInvoicesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/invoices?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<InvoiceDto>($"api/invoices/{id}");
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
            var url = $"api/tax-rates?page={page}&pageSize={pageSize}";
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
            var response = await _http.PostAsJsonAsync("api/tax-rates", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<TaxRateDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<TaxRateDto>();
            return ApiResult<TaxRateDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateTaxRateAsync(Guid id, CreateTaxRateRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/tax-rates/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteTaxRateAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/tax-rates/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Payment Terms ────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<PaymentTermDto>>> GetPaymentTermsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/payment-terms?page={page}&pageSize={pageSize}";
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
            var response = await _http.PostAsJsonAsync("api/payment-terms", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<PaymentTermDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<PaymentTermDto>();
            return ApiResult<PaymentTermDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdatePaymentTermAsync(Guid id, CreatePaymentTermRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/payment-terms/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeletePaymentTermAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/payment-terms/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Promotions ───────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<PromotionDto>>> GetPromotionsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/promotions?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<PromotionDto>($"api/promotions/{id}");
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
            var response = await _http.PostAsJsonAsync("api/promotions", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<PromotionDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<PromotionDto>();
            return ApiResult<PromotionDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdatePromotionAsync(Guid id, CreatePromotionRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/promotions/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeletePromotionAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/promotions/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Coupons ──────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<CouponCodeDto>>> GetCouponsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/coupons?page={page}&pageSize={pageSize}";
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
            var response = await _http.PostAsJsonAsync("api/coupons", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<CouponCodeDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<CouponCodeDto>();
            return ApiResult<CouponCodeDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateCouponAsync(Guid id, CreateCouponCodeRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/coupons/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteCouponAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/coupons/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Loyalty Programs ─────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<LoyaltyProgramDto>>> GetLoyaltyProgramsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/loyalty-programs?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<LoyaltyProgramDto>($"api/loyalty-programs/{id}");
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
            var url = $"api/loyalty-memberships?page={page}&pageSize={pageSize}";
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
            var url = $"api/departments?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<DepartmentDto>($"api/departments/{id}");
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
            var response = await _http.PostAsJsonAsync("api/departments", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<DepartmentDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<DepartmentDto>();
            return ApiResult<DepartmentDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateDepartmentAsync(Guid id, CreateDepartmentRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/departments/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteDepartmentAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/departments/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Employees ────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<EmployeeDto>>> GetEmployeesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/employees?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<EmployeeDto>($"api/employees/{id}");
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
            var response = await _http.PostAsJsonAsync("api/employees", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<EmployeeDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<EmployeeDto>();
            return ApiResult<EmployeeDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateEmployeeAsync(Guid id, CreateEmployeeRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/employees/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteEmployeeAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/employees/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Attendance ───────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<AttendanceDto>>> GetAttendanceAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/attendance?page={page}&pageSize={pageSize}";
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
            var response = await _http.PostAsync("api/attendance/clock-in", null);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> ClockOutAsync()
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync("api/attendance/clock-out", null);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Leave Requests ───────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<LeaveRequestDto>>> GetLeaveRequestsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/leave-requests?page={page}&pageSize={pageSize}";
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
            var response = await _http.PostAsJsonAsync("api/leave-requests", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<LeaveRequestDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<LeaveRequestDto>();
            return ApiResult<LeaveRequestDto>.Ok(data!);
        }

        public async Task<ApiResult> ApproveLeaveRequestAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync($"api/leave-requests/{id}/approve", null);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> RejectLeaveRequestAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync($"api/leave-requests/{id}/reject", null);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Branches ─────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<BranchDto>>> GetBranchesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/branches?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<BranchDto>($"api/branches/{id}");
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
            var response = await _http.PostAsJsonAsync("api/branches", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<BranchDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<BranchDto>();
            return ApiResult<BranchDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateBranchAsync(Guid id, CreateBranchRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/branches/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteBranchAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/branches/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Delivery Zones ───────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<DeliveryZoneDto>>> GetDeliveryZonesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/delivery-zones?page={page}&pageSize={pageSize}";
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
                var data = await _http.GetFromJsonAsync<DeliveryZoneDto>($"api/delivery-zones/{id}");
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
            var response = await _http.PostAsJsonAsync("api/delivery-zones", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<DeliveryZoneDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<DeliveryZoneDto>();
            return ApiResult<DeliveryZoneDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateDeliveryZoneAsync(Guid id, CreateDeliveryZoneRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/delivery-zones/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteDeliveryZoneAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/delivery-zones/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Notifications ────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<NotificationDto>>> GetMyNotificationsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/notifications/my?page={page}&pageSize={pageSize}";
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
            var resp = await _http.GetAsync("api/notifications/unread-count");
            if (!resp.IsSuccessStatusCode)
                return ApiResult<int>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<int>();
            return ApiResult<int>.Ok(data);
        }

        public async Task<ApiResult> MarkNotificationReadAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync($"api/notifications/{id}/read", null);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Tenant Settings ──────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<TenantSettingDto>>> GetTenantSettingsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/tenant-settings?page={page}&pageSize={pageSize}";
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
            var response = await _http.PutAsJsonAsync($"api/tenant-settings/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Reports ──────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<ReportDefinitionDto>>> GetReportsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/reports?page={page}&pageSize={pageSize}";
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
            var url = $"api/audit-logs?page={page}&pageSize={pageSize}";
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
            var url = $"api/api-keys?page={page}&pageSize={pageSize}";
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
            var url = $"api/webhooks?page={page}&pageSize={pageSize}";
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
            var url = $"api/banners?page={page}&pageSize={pageSize}";
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
            var response = await _http.PostAsJsonAsync("api/banners", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<BannerDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<BannerDto>();
            return ApiResult<BannerDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdateBannerAsync(Guid id, CreateBannerRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/banners/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeleteBannerAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/banners/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Pages ────────────────────────────────────────────────────────────────

        public async Task<ApiResult<PagedResult<PageDto>>> GetPagesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/pages?page={page}&pageSize={pageSize}";
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
            var response = await _http.PostAsJsonAsync("api/pages", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult<PageDto>.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            var data = await response.Content.ReadFromJsonAsync<PageDto>();
            return ApiResult<PageDto>.Ok(data!);
        }

        public async Task<ApiResult> UpdatePageAsync(Guid id, CreatePageRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/pages/{id}", request);
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        public async Task<ApiResult> DeletePageAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/pages/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResult.Fail((int)response.StatusCode, await ReadErrorMessageAsync(response));
            return ApiResult.Ok();
        }

        // ── Storefront Config ────────────────────────────────────────────────────

        public async Task<ApiResult<StorefrontConfigDto>> GetStorefrontConfigAsync()
        {
            await AttachTokenAsync();
            var resp = await _http.GetAsync("api/storefront-config");
            if (!resp.IsSuccessStatusCode)
                return ApiResult<StorefrontConfigDto>.Fail((int)resp.StatusCode, await ReadErrorMessageAsync(resp));
            var data = await resp.Content.ReadFromJsonAsync<StorefrontConfigDto>();
            return ApiResult<StorefrontConfigDto>.Ok(data!);
        }
    }
}
