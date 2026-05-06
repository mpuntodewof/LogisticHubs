using Blazored.LocalStorage;
using BlazorApp.Client.Models;
using System.Net.Http.Json;

namespace BlazorApp.Client.Services.ApiClients
{
    public class UserManagementClient : BaseApiClient
    {
        public UserManagementClient(HttpClient http, ILocalStorageService localStorage) : base(http, localStorage) { }

        #region Users

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

        #endregion

        #region Roles

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

        #endregion

        #region Permissions

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

        #endregion
    }
}
