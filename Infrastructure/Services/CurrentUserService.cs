using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

        public Guid? UserId
        {
            get
            {
                var sub = Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? Principal?.FindFirstValue("sub");
                return Guid.TryParse(sub, out var id) ? id : null;
            }
        }

        public Guid? TenantId
        {
            get
            {
                var tenantClaim = Principal?.FindFirstValue("tenant_id");
                return Guid.TryParse(tenantClaim, out var id) ? id : null;
            }
        }

        public string? Email => Principal?.FindFirstValue(ClaimTypes.Email)
            ?? Principal?.FindFirstValue("email");

        public IEnumerable<string> Roles =>
            Principal?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? [];

        public IEnumerable<string> Permissions =>
            Principal?.FindAll("permissions").Select(c => c.Value) ?? [];

        public bool HasPermission(string permission) =>
            Permissions.Contains(permission);

        public bool IsAuthenticated =>
            Principal?.Identity?.IsAuthenticated == true;
    }
}