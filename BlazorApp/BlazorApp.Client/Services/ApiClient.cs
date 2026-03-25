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
    }
}
