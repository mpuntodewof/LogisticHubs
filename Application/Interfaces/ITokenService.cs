using Application.DTOs.Auth;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions, Guid tenantId);
        (string rawToken, string tokenHash) GenerateRefreshToken();
    }
}