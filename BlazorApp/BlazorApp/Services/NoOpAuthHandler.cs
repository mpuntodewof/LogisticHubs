using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace BlazorApp.Services
{
    /// <summary>
    /// No-op authentication handler that always succeeds with an empty principal.
    /// This satisfies the IAuthenticationService requirement without issuing
    /// cookie redirects or any other side effects.
    /// Real auth is handled by JwtAuthStateProvider in the WASM client.
    /// </summary>
    public sealed class NoOpAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public NoOpAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            => Task.FromResult(AuthenticateResult.NoResult());

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
            => Task.CompletedTask; // Do nothing — no redirect

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
            => Task.CompletedTask; // Do nothing — no redirect
    }
}
