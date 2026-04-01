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
            // 1. Try JWT claim (authenticated requests)
            var tenantClaim = context.User?.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(tenantClaim, out var tenantIdFromJwt))
            {
                tenantContext.SetTenantId(tenantIdFromJwt);
                await _next(context);
                return;
            }

            // 2. Fallback to X-Tenant-Id header (register, login, etc.)
            if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var headerValue)
                && Guid.TryParse(headerValue.FirstOrDefault(), out var tenantIdFromHeader))
            {
                tenantContext.SetTenantId(tenantIdFromHeader);
                await _next(context);
                return;
            }

            // 3. No tenant resolved — let request proceed without tenant scope.
            //    Auth endpoints handle tenant resolution explicitly.
            await _next(context);
        }
    }
}
