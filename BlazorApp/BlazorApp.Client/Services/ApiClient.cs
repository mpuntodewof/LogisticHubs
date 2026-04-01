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

        // ── Auth ─────────────────────────────────────────────────────────────────

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", request);
            return response.IsSuccessStatusCode;
        }

        public async Task LogoutAsync(string refreshToken)
        {
            await AttachTokenAsync();
            await _http.PostAsJsonAsync("api/auth/logout", new { refreshToken });
        }

        public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken)
        {
            var response = await _http.PostAsJsonAsync("api/auth/refresh", new { refreshToken });
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        // ── Shipments ────────────────────────────────────────────────────────────

        public async Task<List<ShipmentDto>> GetShipmentsAsync()
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<List<ShipmentDto>>("api/shipments") ?? new();
        }

        public async Task<ShipmentDto?> GetShipmentAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<ShipmentDto>($"api/shipments/{id}");
        }

        public async Task<ShipmentDto?> CreateShipmentAsync(CreateShipmentRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/shipments", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ShipmentDto>();
        }

        public async Task<List<TrackingEventDto>> GetTrackingAsync(Guid shipmentId)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<List<TrackingEventDto>>($"api/shipments/{shipmentId}/tracking") ?? new();
        }

        public async Task<bool> AssignShipmentAsync(Guid id, AssignShipmentRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"api/shipments/{id}/assign", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<TrackingEventDto?> AddTrackingEventAsync(Guid shipmentId, AddTrackingEventRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"api/shipments/{shipmentId}/tracking", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<TrackingEventDto>();
        }

        // ── Drivers ──────────────────────────────────────────────────────────────

        public async Task<List<DriverDto>> GetDriversAsync()
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<List<DriverDto>>("api/drivers") ?? new();
        }

        public async Task<DriverDto?> CreateDriverAsync(CreateDriverRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/drivers", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<DriverDto>();
        }

        public async Task<bool> UpdateDriverAsync(Guid id, UpdateDriverRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/drivers/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDriverAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/drivers/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Vehicles ─────────────────────────────────────────────────────────────

        public async Task<List<VehicleDto>> GetVehiclesAsync()
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<List<VehicleDto>>("api/vehicles") ?? new();
        }

        public async Task<VehicleDto?> CreateVehicleAsync(CreateVehicleRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/vehicles", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<VehicleDto>();
        }

        public async Task<bool> UpdateVehicleAsync(Guid id, UpdateVehicleRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/vehicles/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteVehicleAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/vehicles/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Warehouses ───────────────────────────────────────────────────────────

        public async Task<List<WarehouseDto>> GetWarehousesAsync()
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<List<WarehouseDto>>("api/warehouses") ?? new();
        }

        public async Task<WarehouseDto?> CreateWarehouseAsync(CreateWarehouseRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/warehouses", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<WarehouseDto>();
        }

        public async Task<bool> UpdateWarehouseAsync(Guid id, UpdateWarehouseRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/warehouses/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteWarehouseAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/warehouses/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Users ────────────────────────────────────────────────────────────────

        public async Task<List<UserDto>> GetUsersAsync()
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<List<UserDto>>("api/users") ?? new();
        }

        public async Task<UserDto?> GetUserAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<UserDto>($"api/users/{id}");
        }

        public async Task<bool> CreateUserAsync(CreateUserRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/auth/register", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUserAsync(Guid id, UpdateUserRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/users/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/users/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AssignUserRoleAsync(Guid userId, AssignRoleRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"api/users/{userId}/roles", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RevokeUserRoleAsync(Guid userId, Guid roleId)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/users/{userId}/roles/{roleId}");
            return response.IsSuccessStatusCode;
        }

        // ── Roles ───────────────────────────────────────────────────────────────

        public async Task<List<RoleDto>> GetRolesAsync()
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<List<RoleDto>>("api/roles") ?? new();
        }

        public async Task<RoleDto?> GetRoleAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<RoleDto>($"api/roles/{id}");
        }

        public async Task<RoleDto?> CreateRoleAsync(CreateRoleRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/roles", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<RoleDto>();
        }

        public async Task<bool> UpdateRoleAsync(Guid id, UpdateRoleRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/roles/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRoleAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/roles/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateRolePermissionsAsync(Guid roleId, UpdateRolePermissionsRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/roles/{roleId}/permissions", request);
            return response.IsSuccessStatusCode;
        }

        // ── Permissions ─────────────────────────────────────────────────────────

        public async Task<List<PermissionDto>> GetPermissionsAsync()
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<List<PermissionDto>>("api/permissions") ?? new();
        }

        // ══════════════════════════════════════════════════════════════════════════
        // NEW MODULE API METHODS
        // ══════════════════════════════════════════════════════════════════════════

        // ── Categories ───────────────────────────────────────────────────────────

        public async Task<PagedResult<CategoryDto>?> GetCategoriesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/categories?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<CategoryDto>>();
        }

        public async Task<CategoryDto?> GetCategoryAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<CategoryDto>($"api/categories/{id}");
        }

        public async Task<CategoryDto?> CreateCategoryAsync(CreateCategoryRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/categories", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<CategoryDto>();
        }

        public async Task<bool> UpdateCategoryAsync(Guid id, CreateCategoryRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/categories/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/categories/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Brands ───────────────────────────────────────────────────────────────

        public async Task<PagedResult<BrandDto>?> GetBrandsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/brands?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<BrandDto>>();
        }

        public async Task<BrandDto?> GetBrandAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<BrandDto>($"api/brands/{id}");
        }

        public async Task<BrandDto?> CreateBrandAsync(CreateBrandRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/brands", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<BrandDto>();
        }

        public async Task<bool> UpdateBrandAsync(Guid id, CreateBrandRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/brands/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBrandAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/brands/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Products ─────────────────────────────────────────────────────────────

        public async Task<PagedResult<ProductDto>?> GetProductsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/products?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
        }

        public async Task<ProductDto?> GetProductAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<ProductDto>($"api/products/{id}");
        }

        public async Task<ProductDto?> CreateProductAsync(CreateProductRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/products", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ProductDto>();
        }

        public async Task<bool> UpdateProductAsync(Guid id, CreateProductRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/products/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/products/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Units of Measure ─────────────────────────────────────────────────────

        public async Task<PagedResult<UnitOfMeasureDto>?> GetUnitsOfMeasureAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/units-of-measure?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<UnitOfMeasureDto>>();
        }

        public async Task<UnitOfMeasureDto?> CreateUnitOfMeasureAsync(CreateUnitOfMeasureRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/units-of-measure", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<UnitOfMeasureDto>();
        }

        public async Task<bool> UpdateUnitOfMeasureAsync(Guid id, CreateUnitOfMeasureRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/units-of-measure/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUnitOfMeasureAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/units-of-measure/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Warehouse Stock (Inventory) ──────────────────────────────────────────

        public async Task<PagedResult<object>?> GetWarehouseStockAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/warehouse-stock?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<object>>();
        }

        // ── Stock Movements ──────────────────────────────────────────────────────

        public async Task<PagedResult<object>?> GetStockMovementsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/stock-movements?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<object>>();
        }

        // ── Sales Orders ─────────────────────────────────────────────────────────

        public async Task<PagedResult<SalesOrderDto>?> GetSalesOrdersAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/sales-orders?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<SalesOrderDto>>();
        }

        public async Task<SalesOrderDto?> GetSalesOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<SalesOrderDto>($"api/sales-orders/{id}");
        }

        public async Task<SalesOrderDto?> CreateSalesOrderAsync(CreateSalesOrderRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/sales-orders", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<SalesOrderDto>();
        }

        public async Task<bool> UpdateSalesOrderAsync(Guid id, CreateSalesOrderRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/sales-orders/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteSalesOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/sales-orders/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ConfirmSalesOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync($"api/sales-orders/{id}/confirm", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CancelSalesOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync($"api/sales-orders/{id}/cancel", null);
            return response.IsSuccessStatusCode;
        }

        // ── Customers ────────────────────────────────────────────────────────────

        public async Task<PagedResult<CustomerDto>?> GetCustomersAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/customers?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<CustomerDto>>();
        }

        public async Task<CustomerDto?> GetCustomerAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<CustomerDto>($"api/customers/{id}");
        }

        public async Task<CustomerDto?> CreateCustomerAsync(CreateCustomerRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/customers", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<CustomerDto>();
        }

        public async Task<bool> UpdateCustomerAsync(Guid id, CreateCustomerRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/customers/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/customers/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Customer Groups ──────────────────────────────────────────────────────

        public async Task<PagedResult<CustomerGroupDto>?> GetCustomerGroupsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/customer-groups?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<CustomerGroupDto>>();
        }

        public async Task<CustomerGroupDto?> CreateCustomerGroupAsync(CreateCustomerGroupRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/customer-groups", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<CustomerGroupDto>();
        }

        public async Task<bool> UpdateCustomerGroupAsync(Guid id, CreateCustomerGroupRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/customer-groups/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCustomerGroupAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/customer-groups/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Purchase Orders ──────────────────────────────────────────────────────

        public async Task<PagedResult<PurchaseOrderDto>?> GetPurchaseOrdersAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/purchase-orders?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<PurchaseOrderDto>>();
        }

        public async Task<PurchaseOrderDto?> GetPurchaseOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<PurchaseOrderDto>($"api/purchase-orders/{id}");
        }

        public async Task<PurchaseOrderDto?> CreatePurchaseOrderAsync(object request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/purchase-orders", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<PurchaseOrderDto>();
        }

        public async Task<bool> UpdatePurchaseOrderAsync(Guid id, object request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/purchase-orders/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePurchaseOrderAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/purchase-orders/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Suppliers ────────────────────────────────────────────────────────────

        public async Task<PagedResult<SupplierDto>?> GetSuppliersAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/suppliers?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<SupplierDto>>();
        }

        public async Task<SupplierDto?> GetSupplierAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<SupplierDto>($"api/suppliers/{id}");
        }

        public async Task<SupplierDto?> CreateSupplierAsync(CreateSupplierRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/suppliers", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<SupplierDto>();
        }

        public async Task<bool> UpdateSupplierAsync(Guid id, CreateSupplierRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/suppliers/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteSupplierAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/suppliers/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Goods Receipts ───────────────────────────────────────────────────────

        public async Task<PagedResult<GoodsReceiptDto>?> GetGoodsReceiptsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/goods-receipts?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<GoodsReceiptDto>>();
        }

        public async Task<GoodsReceiptDto?> GetGoodsReceiptAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<GoodsReceiptDto>($"api/goods-receipts/{id}");
        }

        // ── Chart of Accounts ────────────────────────────────────────────────────

        public async Task<PagedResult<ChartOfAccountDto>?> GetChartOfAccountsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/chart-of-accounts?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<ChartOfAccountDto>>();
        }

        public async Task<ChartOfAccountDto?> GetChartOfAccountAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<ChartOfAccountDto>($"api/chart-of-accounts/{id}");
        }

        public async Task<ChartOfAccountDto?> CreateChartOfAccountAsync(CreateChartOfAccountRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/chart-of-accounts", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<ChartOfAccountDto>();
        }

        public async Task<bool> UpdateChartOfAccountAsync(Guid id, CreateChartOfAccountRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/chart-of-accounts/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteChartOfAccountAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/chart-of-accounts/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Journal Entries ──────────────────────────────────────────────────────

        public async Task<PagedResult<JournalEntryDto>?> GetJournalEntriesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/journal-entries?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<JournalEntryDto>>();
        }

        public async Task<JournalEntryDto?> GetJournalEntryAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<JournalEntryDto>($"api/journal-entries/{id}");
        }

        public async Task<JournalEntryDto?> CreateJournalEntryAsync(CreateJournalEntryRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/journal-entries", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<JournalEntryDto>();
        }

        public async Task<bool> DeleteJournalEntryAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/journal-entries/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Invoices ─────────────────────────────────────────────────────────────

        public async Task<PagedResult<InvoiceDto>?> GetInvoicesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/invoices?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<InvoiceDto>>();
        }

        public async Task<InvoiceDto?> GetInvoiceAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<InvoiceDto>($"api/invoices/{id}");
        }

        // ── Tax Rates ────────────────────────────────────────────────────────────

        public async Task<PagedResult<TaxRateDto>?> GetTaxRatesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/tax-rates?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<TaxRateDto>>();
        }

        public async Task<TaxRateDto?> CreateTaxRateAsync(CreateTaxRateRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/tax-rates", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<TaxRateDto>();
        }

        public async Task<bool> UpdateTaxRateAsync(Guid id, CreateTaxRateRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/tax-rates/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTaxRateAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/tax-rates/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Payment Terms ────────────────────────────────────────────────────────

        public async Task<PagedResult<PaymentTermDto>?> GetPaymentTermsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/payment-terms?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<PaymentTermDto>>();
        }

        public async Task<PaymentTermDto?> CreatePaymentTermAsync(CreatePaymentTermRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/payment-terms", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<PaymentTermDto>();
        }

        public async Task<bool> UpdatePaymentTermAsync(Guid id, CreatePaymentTermRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/payment-terms/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePaymentTermAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/payment-terms/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Promotions ───────────────────────────────────────────────────────────

        public async Task<PagedResult<PromotionDto>?> GetPromotionsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/promotions?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<PromotionDto>>();
        }

        public async Task<PromotionDto?> GetPromotionAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<PromotionDto>($"api/promotions/{id}");
        }

        public async Task<PromotionDto?> CreatePromotionAsync(CreatePromotionRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/promotions", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<PromotionDto>();
        }

        public async Task<bool> UpdatePromotionAsync(Guid id, CreatePromotionRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/promotions/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePromotionAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/promotions/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Coupons ──────────────────────────────────────────────────────────────

        public async Task<PagedResult<CouponCodeDto>?> GetCouponsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/coupons?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<CouponCodeDto>>();
        }

        public async Task<CouponCodeDto?> CreateCouponAsync(CreateCouponCodeRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/coupons", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<CouponCodeDto>();
        }

        public async Task<bool> UpdateCouponAsync(Guid id, CreateCouponCodeRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/coupons/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCouponAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/coupons/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Loyalty Programs ─────────────────────────────────────────────────────

        public async Task<PagedResult<LoyaltyProgramDto>?> GetLoyaltyProgramsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/loyalty-programs?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<LoyaltyProgramDto>>();
        }

        public async Task<LoyaltyProgramDto?> GetLoyaltyProgramAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<LoyaltyProgramDto>($"api/loyalty-programs/{id}");
        }

        // ── Loyalty Memberships ──────────────────────────────────────────────────

        public async Task<PagedResult<LoyaltyMembershipDto>?> GetLoyaltyMembershipsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/loyalty-memberships?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<LoyaltyMembershipDto>>();
        }

        // ── Departments ──────────────────────────────────────────────────────────

        public async Task<PagedResult<DepartmentDto>?> GetDepartmentsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/departments?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<DepartmentDto>>();
        }

        public async Task<DepartmentDto?> GetDepartmentAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<DepartmentDto>($"api/departments/{id}");
        }

        public async Task<DepartmentDto?> CreateDepartmentAsync(CreateDepartmentRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/departments", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<DepartmentDto>();
        }

        public async Task<bool> UpdateDepartmentAsync(Guid id, CreateDepartmentRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/departments/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDepartmentAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/departments/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Employees ────────────────────────────────────────────────────────────

        public async Task<PagedResult<EmployeeDto>?> GetEmployeesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/employees?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<EmployeeDto>>();
        }

        public async Task<EmployeeDto?> GetEmployeeAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<EmployeeDto>($"api/employees/{id}");
        }

        public async Task<EmployeeDto?> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/employees", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<EmployeeDto>();
        }

        public async Task<bool> UpdateEmployeeAsync(Guid id, CreateEmployeeRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/employees/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteEmployeeAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/employees/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Attendance ───────────────────────────────────────────────────────────

        public async Task<PagedResult<AttendanceDto>?> GetAttendanceAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/attendance?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<AttendanceDto>>();
        }

        public async Task<bool> ClockInAsync()
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync("api/attendance/clock-in", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ClockOutAsync()
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync("api/attendance/clock-out", null);
            return response.IsSuccessStatusCode;
        }

        // ── Leave Requests ───────────────────────────────────────────────────────

        public async Task<PagedResult<LeaveRequestDto>?> GetLeaveRequestsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/leave-requests?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<LeaveRequestDto>>();
        }

        public async Task<LeaveRequestDto?> CreateLeaveRequestAsync(CreateLeaveRequestRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/leave-requests", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<LeaveRequestDto>();
        }

        public async Task<bool> ApproveLeaveRequestAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync($"api/leave-requests/{id}/approve", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RejectLeaveRequestAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync($"api/leave-requests/{id}/reject", null);
            return response.IsSuccessStatusCode;
        }

        // ── Branches ─────────────────────────────────────────────────────────────

        public async Task<PagedResult<BranchDto>?> GetBranchesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/branches?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<BranchDto>>();
        }

        public async Task<BranchDto?> GetBranchAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<BranchDto>($"api/branches/{id}");
        }

        public async Task<BranchDto?> CreateBranchAsync(CreateBranchRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/branches", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<BranchDto>();
        }

        public async Task<bool> UpdateBranchAsync(Guid id, CreateBranchRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/branches/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBranchAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/branches/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Delivery Zones ───────────────────────────────────────────────────────

        public async Task<PagedResult<DeliveryZoneDto>?> GetDeliveryZonesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/delivery-zones?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<DeliveryZoneDto>>();
        }

        public async Task<DeliveryZoneDto?> GetDeliveryZoneAsync(Guid id)
        {
            await AttachTokenAsync();
            return await _http.GetFromJsonAsync<DeliveryZoneDto>($"api/delivery-zones/{id}");
        }

        public async Task<DeliveryZoneDto?> CreateDeliveryZoneAsync(CreateDeliveryZoneRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/delivery-zones", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<DeliveryZoneDto>();
        }

        public async Task<bool> UpdateDeliveryZoneAsync(Guid id, CreateDeliveryZoneRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/delivery-zones/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDeliveryZoneAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/delivery-zones/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Notifications ────────────────────────────────────────────────────────

        public async Task<PagedResult<NotificationDto>?> GetMyNotificationsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/notifications/my?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<NotificationDto>>();
        }

        public async Task<int> GetUnreadCountAsync()
        {
            await AttachTokenAsync();
            var resp = await _http.GetAsync("api/notifications/unread-count");
            if (!resp.IsSuccessStatusCode) return 0;
            return await resp.Content.ReadFromJsonAsync<int>();
        }

        public async Task<bool> MarkNotificationReadAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsync($"api/notifications/{id}/read", null);
            return response.IsSuccessStatusCode;
        }

        // ── Tenant Settings ──────────────────────────────────────────────────────

        public async Task<PagedResult<TenantSettingDto>?> GetTenantSettingsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/tenant-settings?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<TenantSettingDto>>();
        }

        public async Task<bool> UpdateTenantSettingAsync(Guid id, object request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/tenant-settings/{id}", request);
            return response.IsSuccessStatusCode;
        }

        // ── Reports ──────────────────────────────────────────────────────────────

        public async Task<PagedResult<ReportDefinitionDto>?> GetReportsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/reports?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<ReportDefinitionDto>>();
        }

        // ── Audit Logs ───────────────────────────────────────────────────────────

        public async Task<PagedResult<AuditLogDto>?> GetAuditLogsAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/audit-logs?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<AuditLogDto>>();
        }

        // ── API Keys ─────────────────────────────────────────────────────────────

        public async Task<PagedResult<ApiKeyDto>?> GetApiKeysAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/api-keys?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<ApiKeyDto>>();
        }

        // ── Webhooks ─────────────────────────────────────────────────────────────

        public async Task<PagedResult<WebhookSubscriptionDto>?> GetWebhooksAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/webhooks?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<WebhookSubscriptionDto>>();
        }

        // ── Banners ──────────────────────────────────────────────────────────────

        public async Task<PagedResult<BannerDto>?> GetBannersAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/banners?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<BannerDto>>();
        }

        public async Task<BannerDto?> CreateBannerAsync(CreateBannerRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/banners", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<BannerDto>();
        }

        public async Task<bool> UpdateBannerAsync(Guid id, CreateBannerRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/banners/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBannerAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/banners/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Pages ────────────────────────────────────────────────────────────────

        public async Task<PagedResult<PageDto>?> GetPagesAsync(int page = 1, int pageSize = 20, string? search = null)
        {
            await AttachTokenAsync();
            var url = $"api/pages?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(search)) url += $"&search={Uri.EscapeDataString(search)}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<PagedResult<PageDto>>();
        }

        public async Task<PageDto?> CreatePageAsync(CreatePageRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/pages", request);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<PageDto>();
        }

        public async Task<bool> UpdatePageAsync(Guid id, CreatePageRequest request)
        {
            await AttachTokenAsync();
            var response = await _http.PutAsJsonAsync($"api/pages/{id}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePageAsync(Guid id)
        {
            await AttachTokenAsync();
            var response = await _http.DeleteAsync($"api/pages/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Storefront Config ────────────────────────────────────────────────────

        public async Task<StorefrontConfigDto?> GetStorefrontConfigAsync()
        {
            await AttachTokenAsync();
            var resp = await _http.GetAsync("api/storefront-config");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<StorefrontConfigDto>();
        }
    }
}
