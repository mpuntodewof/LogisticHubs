using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorApp.Client.Services
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

        public JwtAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("access_token");
            if (string.IsNullOrEmpty(token))
                return _anonymous;

            var claims = ParseClaimsFromJwt(token);
            if (!claims.Any())
                return _anonymous;

            // Check if token is expired
            var expClaim = claims.FirstOrDefault(c => c.Type == "exp");
            if (expClaim != null && long.TryParse(expClaim.Value, out var exp))
            {
                var expDate = DateTimeOffset.FromUnixTimeSeconds(exp);
                if (expDate <= DateTimeOffset.UtcNow)
                    return _anonymous;
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void NotifyUserAuthentication(string token)
        {
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
        }

        /// <summary>Get all permission claims from the current user's JWT.</summary>
        public async Task<IEnumerable<string>> GetPermissionsAsync()
        {
            var state = await GetAuthenticationStateAsync();
            return state.User.Claims
                .Where(c => c.Type == "permissions")
                .Select(c => c.Value);
        }

        /// <summary>Check if the current user has a specific permission.</summary>
        public async Task<bool> HasPermissionAsync(string permission)
        {
            var permissions = await GetPermissionsAsync();
            return permissions.Contains(permission);
        }

        /// <summary>Get all role claims from the current user's JWT.</summary>
        public async Task<IEnumerable<string>> GetRolesAsync()
        {
            var state = await GetAuthenticationStateAsync();
            return state.User.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Select(c => c.Value);
        }

        /// <summary>
        /// Landing route for the currently authenticated user. Currently always
        /// /dashboard — the page itself renders a role-appropriate view
        /// (finance-only for Accountants, inventory-first for everyone else).
        /// </summary>
        public Task<string> GetDefaultLandingRouteAsync() => Task.FromResult("/dashboard");

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var token = handler.ReadJwtToken(jwt);
                return token.Claims;
            }
            catch
            {
                return Enumerable.Empty<Claim>();
            }
        }
    }
}
