using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User> CreateUserAsync(User user);
        Task AssignRoleToUserAsync(Guid userId, Guid roleId);
        Task<IEnumerable<string>> GetUserRoleNamesAsync(Guid userId);
        Task<IEnumerable<string>> GetUserPermissionNamesAsync(Guid userId);
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task SaveRefreshTokenAsync(RefreshToken token);
        Task<RefreshToken?> GetActiveRefreshTokenByHashAsync(string tokenHash);
        Task RevokeRefreshTokenAsync(RefreshToken token, string? revokedByIp, string? replacedByToken);
        Task RevokeAllUserRefreshTokensAsync(Guid userId, string? revokedByIp);

        // Multi-tenancy
        Task<Tenant?> GetTenantByIdAsync(Guid tenantId);
        Task<Tenant> CreateTenantAsync(Tenant tenant);
        Task<Role?> GetRoleByNameAndTenantAsync(string roleName, Guid tenantId);
        Task SeedRolesAndPermissionsForTenantAsync(Guid tenantId);
        Task<User?> GetUserByEmailUnfilteredAsync(string email);
    }
}