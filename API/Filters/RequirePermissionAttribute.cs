using Microsoft.AspNetCore.Authorization;

namespace API.Filters
{
    /// <summary>
    /// Enforces a specific permission claim on an endpoint.
    /// The permission must be present in the JWT "permissions" claim array.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public RequirePermissionAttribute(string permission)
            : base(policy: $"Permission:{permission}")
        {
        }
    }
}