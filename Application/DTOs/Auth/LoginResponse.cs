namespace Application.DTOs.Auth
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; } // seconds
        public string TokenType { get; set; } = "Bearer";
        public UserInfo User { get; set; } = null!;
    }

    public class UserInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = [];
        public Guid TenantId { get; set; }
    }
}