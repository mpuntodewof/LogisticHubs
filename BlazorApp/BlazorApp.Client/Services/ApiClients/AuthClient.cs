using Blazored.LocalStorage;
using BlazorApp.Client.Models;
using System.Net.Http.Json;

namespace BlazorApp.Client.Services.ApiClients
{
    public class AuthClient : BaseApiClient
    {
        public AuthClient(HttpClient http, ILocalStorageService localStorage) : base(http, localStorage) { }

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
    }
}
