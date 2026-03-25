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
            await _context.SaveChangesAsync();
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
            await _context.SaveChangesAsync();
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
            await _context.SaveChangesAsync();
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
            await _context.SaveChangesAsync();
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

            await _context.SaveChangesAsync();
        }
    }
}