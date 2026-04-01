using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllWithPermissionsAsync();
        Task<PagedResult<Role>> GetPagedAsync(PagedRequest request);
        Task<Role?> GetByIdWithPermissionsAsync(Guid id);
        Task<Role?> GetByNameAsync(string name);
        Task<Role> CreateAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(Role role);
        Task SetRolePermissionsAsync(Guid roleId, List<Guid> permissionIds);
        Task<IEnumerable<Permission>> GetAllPermissionsAsync();
    }
}
