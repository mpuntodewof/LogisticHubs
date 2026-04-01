using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.UseCases.Auth
{
    public class AuthUseCase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly ITenantContext _tenantContext;

        public AuthUseCase(
            IAuthRepository authRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            ITenantContext tenantContext)
        {
            _authRepository = authRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _tenantContext = tenantContext;
        }

        // ── Register ────────────────────────────────────────────────────────────

        public async Task<Guid> RegisterAsync(RegisterRequest request)
        {
            Guid tenantId;
            bool isNewTenant = false;

            if (request.TenantId.HasValue)
            {
                // Join existing tenant
                var tenant = await _authRepository.GetTenantByIdAsync(request.TenantId.Value);
                if (tenant == null || !tenant.IsActive)
                    throw new InvalidOperationException("Tenant not found or inactive.");
                tenantId = tenant.Id;
            }
            else if (!string.IsNullOrWhiteSpace(request.CompanyName))
            {
                // Create new tenant
                var slug = Regex.Replace(request.CompanyName.ToLowerInvariant().Replace(" ", "-"), "[^a-z0-9-]", "");
                if (string.IsNullOrWhiteSpace(slug)) slug = Guid.NewGuid().ToString("N")[..8];

                var newTenant = new Tenant
                {
                    Id = Guid.NewGuid(),
                    Slug = slug,
                    CompanyName = request.CompanyName.Trim(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _authRepository.CreateTenantAsync(newTenant);
                tenantId = newTenant.Id;
                isNewTenant = true;

                // Seed default roles and permissions for the new tenant
                _tenantContext.SetTenantId(tenantId);
                await _authRepository.SeedRolesAndPermissionsForTenantAsync(tenantId);
            }
            else
            {
                // Default tenant (backward compatibility)
                tenantId = TenantConstants.DefaultTenantId;
            }

            // Set tenant context for subsequent operations
            if (_tenantContext.TenantId == null)
                _tenantContext.SetTenantId(tenantId);

            // Check email uniqueness within tenant
            var existing = await _authRepository.GetUserByEmailAsync(request.Email.ToLowerInvariant());
            if (existing != null)
                throw new InvalidOperationException("A user with this email already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email.ToLowerInvariant(),
                PasswordHash = _passwordHasher.Hash(request.Password),
                IsActive = true,
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _authRepository.CreateUserAsync(user);

            // Assign default role(s)
            if (isNewTenant)
            {
                // New tenant creator gets Admin role
                var adminRole = await _authRepository.GetRoleByNameAndTenantAsync("Admin", tenantId);
                if (adminRole != null)
                    await _authRepository.AssignRoleToUserAsync(created.Id, adminRole.Id);
            }
            else
            {
                // Joining existing tenant gets Viewer role
                var viewerRole = await _authRepository.GetRoleByNameAndTenantAsync("Viewer", tenantId);
                if (viewerRole != null)
                    await _authRepository.AssignRoleToUserAsync(created.Id, viewerRole.Id);
            }

            return created.Id;
        }

        // ── Login ────────────────────────────────────────────────────────────────

        public async Task<LoginResponse> LoginAsync(LoginRequest request, string? ipAddress)
        {
            // Use unfiltered query — we don't know the tenant yet
            var user = await _authRepository.GetUserByEmailUnfilteredAsync(request.Email.ToLowerInvariant());

            if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is disabled.");

            // Set tenant context from the user's tenant
            _tenantContext.SetTenantId(user.TenantId);

            return await BuildLoginResponseAsync(user, ipAddress);
        }

        // ── Refresh Token ────────────────────────────────────────────────────────

        public async Task<LoginResponse> RefreshTokenAsync(string rawRefreshToken, string? ipAddress)
        {
            var tokenHash = HashToken(rawRefreshToken);

            // Refresh tokens are globally unique, use unfiltered lookup
            var existing = await _authRepository.GetActiveRefreshTokenByHashAsync(tokenHash);

            if (existing == null)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var user = await _authRepository.GetUserByIdAsync(existing.UserId);
            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("User not found or disabled.");

            // Ensure tenant context is set
            if (_tenantContext.TenantId == null)
                _tenantContext.SetTenantId(user.TenantId);

            var roles = (await _authRepository.GetUserRoleNamesAsync(user.Id)).ToList();
            var permissions = (await _authRepository.GetUserPermissionNamesAsync(user.Id)).ToList();

            var (newRawToken, newTokenHash) = _tokenService.GenerateRefreshToken();

            await _authRepository.RevokeRefreshTokenAsync(existing, ipAddress, newTokenHash);

            await _authRepository.SaveRefreshTokenAsync(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = newTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                TenantId = user.TenantId
            });

            return new LoginResponse
            {
                AccessToken = _tokenService.GenerateAccessToken(user, roles, permissions, user.TenantId),
                RefreshToken = newRawToken,
                ExpiresIn = 900,
                User = new UserInfo { Id = user.Id, Name = user.Name, Email = user.Email, Roles = roles, TenantId = user.TenantId }
            };
        }

        // ── Logout ───────────────────────────────────────────────────────────────

        public async Task LogoutAsync(string rawRefreshToken, string? ipAddress)
        {
            var tokenHash = HashToken(rawRefreshToken);
            var token = await _authRepository.GetActiveRefreshTokenByHashAsync(tokenHash);

            if (token != null)
                await _authRepository.RevokeRefreshTokenAsync(token, ipAddress, null);
        }

        public async Task LogoutAllAsync(Guid userId, string? ipAddress)
        {
            await _authRepository.RevokeAllUserRefreshTokensAsync(userId, ipAddress);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private async Task<LoginResponse> BuildLoginResponseAsync(User user, string? ipAddress)
        {
            var roles = (await _authRepository.GetUserRoleNamesAsync(user.Id)).ToList();
            var permissions = (await _authRepository.GetUserPermissionNamesAsync(user.Id)).ToList();

            var (rawRefreshToken, tokenHash) = _tokenService.GenerateRefreshToken();

            await _authRepository.SaveRefreshTokenAsync(new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                TenantId = user.TenantId
            });

            return new LoginResponse
            {
                AccessToken = _tokenService.GenerateAccessToken(user, roles, permissions, user.TenantId),
                RefreshToken = rawRefreshToken,
                ExpiresIn = 900,
                User = new UserInfo { Id = user.Id, Name = user.Name, Email = user.Email, Roles = roles, TenantId = user.TenantId }
            };
        }

        private static string HashToken(string rawToken)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
            return Convert.ToBase64String(bytes);
        }
    }
}
