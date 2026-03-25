using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByIdWithRolesAsync(Guid id);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task AssignRoleAsync(Guid userId, Guid roleId, Guid? assignedBy);
        Task RevokeRoleAsync(Guid userId, Guid roleId);
        Task<Role?> GetRoleByIdAsync(Guid roleId);
        Task<bool> UserHasRoleAsync(Guid userId, Guid roleId);
    }
}