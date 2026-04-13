using Blazored.LocalStorage;
using BlazorApp.Client.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace BlazorApp.Client.Services
{
    public class AuthService : IAsyncDisposable
    {
        private readonly ApiClient _api;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;
        private Timer? _refreshTimer;

        public AuthService(ApiClient api, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _api = api;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> LoginAsync(LoginRequest request)
        {
            var result = await _api.LoginAsync(request);
            if (!result.Success || result.Data == null) return false;

            await _localStorage.SetItemAsync("access_token", result.Data.AccessToken);
            await _localStorage.SetItemAsync("refresh_token", result.Data.RefreshToken);
            await _localStorage.SetItemAsync("user_info", result.Data.User);

            ((JwtAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Data.AccessToken);
            StartRefreshTimer(result.Data.AccessToken);
            return true;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var result = await _api.RegisterAsync(request);
            return result.Success;
        }

        public async Task LogoutAsync()
        {
            StopRefreshTimer();
            var refreshToken = await _localStorage.GetItemAsync<string>("refresh_token") ?? string.Empty;
            try { await _api.LogoutAsync(refreshToken); } catch { }

            await _localStorage.RemoveItemAsync("access_token");
            await _localStorage.RemoveItemAsync("refresh_token");
            await _localStorage.RemoveItemAsync("user_info");

            ((JwtAuthStateProvider)_authStateProvider).NotifyUserLogout();
        }

        public async Task<UserInfo?> GetCurrentUserAsync()
        {
            return await _localStorage.GetItemAsync<UserInfo>("user_info");
        }

        /// <summary>Try to refresh the access token using the stored refresh token.</summary>
        public async Task<bool> TryRefreshTokenAsync()
        {
            var refreshToken = await _localStorage.GetItemAsync<string>("refresh_token");
            if (string.IsNullOrEmpty(refreshToken)) return false;

            var result = await _api.RefreshTokenAsync(refreshToken);
            if (!result.Success || result.Data == null) return false;

            await _localStorage.SetItemAsync("access_token", result.Data.AccessToken);
            await _localStorage.SetItemAsync("refresh_token", result.Data.RefreshToken);
            await _localStorage.SetItemAsync("user_info", result.Data.User);

            ((JwtAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Data.AccessToken);
            StartRefreshTimer(result.Data.AccessToken);
            return true;
        }

        /// <summary>Start a timer that refreshes the token 2 minutes before expiry.</summary>
        private void StartRefreshTimer(string accessToken)
        {
            StopRefreshTimer();

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(accessToken);
                var exp = jwt.ValidTo;
                var refreshAt = exp.AddMinutes(-2) - DateTime.UtcNow;

                if (refreshAt.TotalSeconds < 10)
                    refreshAt = TimeSpan.FromSeconds(10);

                _refreshTimer = new Timer(async _ =>
                {
                    try { await TryRefreshTokenAsync(); }
                    catch { /* token refresh failed — user will be redirected on next API call */ }
                }, null, refreshAt, Timeout.InfiniteTimeSpan);
            }
            catch { /* invalid token — skip timer */ }
        }

        private void StopRefreshTimer()
        {
            _refreshTimer?.Dispose();
            _refreshTimer = null;
        }

        public ValueTask DisposeAsync()
        {
            StopRefreshTimer();
            return ValueTask.CompletedTask;
        }
    }
}
