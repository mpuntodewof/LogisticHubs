using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllWithPermissionsAsync()
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<PagedResult<Role>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(r => r.Name.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name),
                _ => query.OrderBy(r => r.Name)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<Role?> GetByIdWithPermissionsAsync(Guid id)
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<Role> CreateAsync(Role role)
        {
            _context.Roles.Add(role);
            return role;
        }

        public async Task UpdateAsync(Role role)
        {
            _context.Roles.Update(role);
        }

        public async Task DeleteAsync(Role role)
        {
            _context.Roles.Remove(role);
        }

        public async Task SetRolePermissionsAsync(Guid roleId, List<Guid> permissionIds)
        {
            var existing = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            _context.RolePermissions.RemoveRange(existing);

            foreach (var permissionId in permissionIds)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                });
            }

        }

        public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
        {
            return await _context.Permissions
                .OrderBy(p => p.Resource)
                .ThenBy(p => p.Action)
                .ToListAsync();
        }
    }
}
