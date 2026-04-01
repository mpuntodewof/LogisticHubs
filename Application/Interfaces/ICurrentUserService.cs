namespace Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Guid? TenantId { get; }
        string? Email { get; }
        IEnumerable<string> Roles { get; }
        IEnumerable<string> Permissions { get; }
        bool HasPermission(string permission);
        bool IsAuthenticated { get; }
    }
}