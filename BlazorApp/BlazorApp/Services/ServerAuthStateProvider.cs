using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorApp.Services
{
    /// <summary>
    /// Server-side auth state provider that returns a fully-authorized pass-through user.
    /// During SSR, we can't access localStorage/JS interop, so we let all pages render.
    /// Once WASM boots, JwtAuthStateProvider takes over with real JWT-based authorization.
    /// </summary>
    public sealed class ServerAuthStateProvider : AuthenticationStateProvider
    {
        private static readonly AuthenticationState PassThrough;

        static ServerAuthStateProvider()
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "SSR-PassThrough"),
            };

            // Add all permission claims so policy-based [Authorize] doesn't block SSR
            var allPermissions = new[]
            {
                "users.create", "users.read", "users.update", "users.delete",
                "roles.assign",
                "shipments.create", "shipments.read", "shipments.update", "shipments.delete", "shipments.assign",
                "tracking.create", "tracking.read",
                "drivers.manage", "vehicles.manage", "warehouses.manage",
                "roles.create", "roles.read", "roles.update", "roles.delete"
            };

            foreach (var p in allPermissions)
                claims.Add(new Claim("permissions", p));

            var identity = new ClaimsIdentity(claims, authenticationType: "ServerPassThrough");
            PassThrough = new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
            => Task.FromResult(PassThrough);
    }
}
