using Domain.Interfaces;

namespace API.Middleware
{
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
        {
            // 1. Try JWT claim (authenticated requests) — this is the trusted source
            var tenantClaim = context.User?.FindFirst("tenant_id")?.Value;
            var hasTenantFromJwt = Guid.TryParse(tenantClaim, out var tenantIdFromJwt);

            if (hasTenantFromJwt)
            {
                // If an X-Tenant-Id header is also present, it must match the JWT claim.
                // Reject mismatches to prevent cross-tenant probing.
                if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var headerValue)
                    && Guid.TryParse(headerValue.FirstOrDefault(), out var tenantIdFromHeader)
                    && tenantIdFromHeader != tenantIdFromJwt)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { error = "Tenant mismatch between token and header." });
                    return;
                }

                tenantContext.SetTenantId(tenantIdFromJwt);
                await _next(context);
                return;
            }

            // 2. Fallback to X-Tenant-Id header (unauthenticated requests: register, login, etc.)
            if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var fallbackHeader)
                && Guid.TryParse(fallbackHeader.FirstOrDefault(), out var tenantIdFromFallback))
            {
                tenantContext.SetTenantId(tenantIdFromFallback);
                await _next(context);
                return;
            }

            // 3. No tenant resolved — let request proceed without tenant scope.
            //    Auth endpoints handle tenant resolution explicitly.
            await _next(context);
        }
    }
}
