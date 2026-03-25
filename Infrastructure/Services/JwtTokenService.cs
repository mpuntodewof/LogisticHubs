using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Name, user.Name),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            foreach (var permission in permissions)
                claims.Add(new Claim("permissions", permission));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"] ?? "logistichub-api",
                audience: _configuration["JwtSettings:Audience"] ?? "logistichub-client",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (string rawToken, string tokenHash) GenerateRefreshToken()
        {
            var rawBytes = RandomNumberGenerator.GetBytes(64);
            var rawToken = Convert.ToBase64String(rawBytes);
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
            var tokenHash = Convert.ToBase64String(hashBytes);
            return (rawToken, tokenHash);
        }
    }
}