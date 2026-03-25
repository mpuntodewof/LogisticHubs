using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Application.UseCases.Auth
{
    public class AuthUseCase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthUseCase(
            IAuthRepository authRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _authRepository = authRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        // ── Register ────────────────────────────────────────────────────────────

        public async Task<Guid> RegisterAsync(RegisterRequest request)
        {
            var existing = await _authRepository.GetUserByEmailAsync(request.Email);
            if (existing != null)
                throw new InvalidOperationException("A user with this email already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email.ToLowerInvariant(),
                PasswordHash = _passwordHasher.Hash(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _authRepository.CreateUserAsync(user);

            var viewerRole = await _authRepository.GetRoleByNameAsync("Viewer");
            if (viewerRole != null)
                await _authRepository.AssignRoleToUserAsync(created.Id, viewerRole.Id);

            return created.Id;
        }

        // ── Login ────────────────────────────────────────────────────────────────

        public async Task<LoginResponse> LoginAsync(LoginRequest request, string? ipAddress)
        {
            var user = await _authRepository.GetUserByEmailAsync(request.Email.ToLowerInvariant());

            if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is disabled.");

            return await BuildLoginResponseAsync(user, ipAddress);
        }

        // ── Refresh Token ────────────────────────────────────────────────────────

        public async Task<LoginResponse> RefreshTokenAsync(string rawRefreshToken, string? ipAddress)
        {
            var tokenHash = HashToken(rawRefreshToken);
            var existing = await _authRepository.GetActiveRefreshTokenByHashAsync(tokenHash);

            if (existing == null)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var user = await _authRepository.GetUserByIdAsync(existing.UserId);
            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("User not found or disabled.");

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
                CreatedByIp = ipAddress
            });

            return new LoginResponse
            {
                AccessToken = _tokenService.GenerateAccessToken(user, roles, permissions),
                RefreshToken = newRawToken,
                ExpiresIn = 900,
                User = new UserInfo { Id = user.Id, Name = user.Name, Email = user.Email, Roles = roles }
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
                CreatedByIp = ipAddress
            });

            return new LoginResponse
            {
                AccessToken = _tokenService.GenerateAccessToken(user, roles, permissions),
                RefreshToken = rawRefreshToken,
                ExpiresIn = 900,
                User = new UserInfo { Id = user.Id, Name = user.Name, Email = user.Email, Roles = roles }
            };
        }

        private static string HashToken(string rawToken)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
            return Convert.ToBase64String(bytes);
        }
    }
}
