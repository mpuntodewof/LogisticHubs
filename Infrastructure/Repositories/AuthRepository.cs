using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            return user;
        }

        public async Task AssignRoleToUserAsync(Guid userId, Guid roleId)
        {
            var assignment = new UserRoleAssignment
            {
                UserId = userId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow
            };
            _context.UserRoleAssignments.Add(assignment);
        }

        public async Task<IEnumerable<string>> GetUserRoleNamesAsync(Guid userId)
        {
            return await _context.UserRoleAssignments
                .Where(ura => ura.UserId == userId)
                .Include(ura => ura.Role)
                .Select(ura => ura.Role.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetUserPermissionNamesAsync(Guid userId)
        {
            return await _context.UserRoleAssignments
                .Where(ura => ura.UserId == userId)
                .Include(ura => ura.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .SelectMany(ura => ura.Role.RolePermissions.Select(rp => rp.Permission.Name))
                .Distinct()
                .ToListAsync();
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task SaveRefreshTokenAsync(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
        }

        public async Task<RefreshToken?> GetActiveRefreshTokenByHashAsync(string tokenHash)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt =>
                    rt.TokenHash == tokenHash &&
                    rt.RevokedAt == null &&
                    rt.ExpiresAt > DateTime.UtcNow);
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken token, string? revokedByIp, string? replacedByToken)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = revokedByIp;
            token.ReplacedByToken = replacedByToken;
        }

        public async Task RevokeAllUserRefreshTokensAsync(Guid userId, string? revokedByIp)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync();

            var now = DateTime.UtcNow;
            foreach (var token in tokens)
            {
                token.RevokedAt = now;
                token.RevokedByIp = revokedByIp;
            }

        }

        // ── Multi-tenancy methods ───────────────────────────────────────────

        public async Task<Tenant?> GetTenantByIdAsync(Guid tenantId)
        {
            return await _context.Tenants.FindAsync(tenantId);
        }

        public async Task<Tenant> CreateTenantAsync(Tenant tenant)
        {
            _context.Tenants.Add(tenant);
            return tenant;
        }

        public async Task<Role?> GetRoleByNameAndTenantAsync(string roleName, Guid tenantId)
        {
            return await _context.Roles
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.Name == roleName && r.TenantId == tenantId);
        }

        public async Task<User?> GetUserByEmailUnfilteredAsync(string email)
        {
            return await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task SeedRolesAndPermissionsForTenantAsync(Guid tenantId)
        {
            var permDefinitions = new[]
            {
                ("users.create",      "users",      "create"),
                ("users.read",        "users",      "read"),
                ("users.update",      "users",      "update"),
                ("users.delete",      "users",      "delete"),
                ("roles.assign",      "roles",      "assign"),
                ("shipments.create",  "shipments",  "create"),
                ("shipments.read",    "shipments",  "read"),
                ("shipments.update",  "shipments",  "update"),
                ("shipments.delete",  "shipments",  "delete"),
                ("shipments.assign",  "shipments",  "assign"),
                ("tracking.create",   "tracking",   "create"),
                ("tracking.read",     "tracking",   "read"),
                ("drivers.manage",    "drivers",    "manage"),
                ("vehicles.manage",   "vehicles",   "manage"),
                ("warehouses.manage", "warehouses", "manage"),
                ("roles.create",      "roles",      "create"),
                ("roles.read",        "roles",      "read"),
                ("roles.update",      "roles",      "update"),
                ("roles.delete",      "roles",      "delete"),
            };

            // Create permissions
            var permIds = new Dictionary<string, Guid>();
            foreach (var (name, resource, action) in permDefinitions)
            {
                var perm = new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Resource = resource,
                    Action = action,
                    TenantId = tenantId
                };
                _context.Permissions.Add(perm);
                permIds[name] = perm.Id;
            }

            // Create roles
            var adminRole = new Role { Id = Guid.NewGuid(), Name = "Admin", Description = "Full system access", IsSystem = true, TenantId = tenantId };
            var managerRole = new Role { Id = Guid.NewGuid(), Name = "Manager", Description = "Manage shipments, drivers, vehicles", IsSystem = true, TenantId = tenantId };
            var driverRole = new Role { Id = Guid.NewGuid(), Name = "Driver", Description = "View assignments, update tracking", IsSystem = true, TenantId = tenantId };
            var viewerRole = new Role { Id = Guid.NewGuid(), Name = "Viewer", Description = "Read-only access", IsSystem = true, TenantId = tenantId };

            _context.Roles.AddRange(adminRole, managerRole, driverRole, viewerRole);

            // Assign permissions to roles
            foreach (var permId in permIds.Values)
                _context.RolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = permId, TenantId = tenantId });

            foreach (var p in new[] { "users.read", "shipments.create", "shipments.read", "shipments.update", "shipments.assign", "tracking.create", "tracking.read", "drivers.manage", "vehicles.manage", "warehouses.manage" })
                _context.RolePermissions.Add(new RolePermission { RoleId = managerRole.Id, PermissionId = permIds[p], TenantId = tenantId });

            foreach (var p in new[] { "shipments.read", "tracking.create", "tracking.read" })
                _context.RolePermissions.Add(new RolePermission { RoleId = driverRole.Id, PermissionId = permIds[p], TenantId = tenantId });

            foreach (var p in new[] { "shipments.read", "tracking.read" })
                _context.RolePermissions.Add(new RolePermission { RoleId = viewerRole.Id, PermissionId = permIds[p], TenantId = tenantId });

        }
    }
}