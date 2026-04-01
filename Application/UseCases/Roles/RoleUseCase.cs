using Application.DTOs.Common;
using Application.DTOs.Roles;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Roles
{
    public class RoleUseCase
    {
        private readonly IRoleRepository _roleRepository;

        public RoleUseCase(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllWithPermissionsAsync();
            return roles.Select(MapToDto);
        }

        public async Task<PagedResult<RoleDto>> GetPagedAsync(PagedRequest request)
        {
            var paged = await _roleRepository.GetPagedAsync(request);
            return new PagedResult<RoleDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<RoleDto?> GetByIdAsync(Guid id)
        {
            var role = await _roleRepository.GetByIdWithPermissionsAsync(id);
            return role == null ? null : MapToDto(role);
        }

        // ── Create ──────────────────────────────────────────────────────────────

        public async Task<RoleDto> CreateAsync(CreateRoleRequest request)
        {
            var existing = await _roleRepository.GetByNameAsync(request.Name);
            if (existing != null)
                throw new InvalidOperationException($"A role with name '{request.Name}' already exists.");

            var role = new Role
            {
                Name = request.Name,
                Description = request.Description,
                IsSystem = false
            };

            var created = await _roleRepository.CreateAsync(role);
            return MapToDto(created);
        }

        // ── Update ──────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateRoleRequest request)
        {
            var role = await _roleRepository.GetByIdWithPermissionsAsync(id)
                ?? throw new KeyNotFoundException($"Role {id} not found.");

            if (request.Name != null)
            {
                if (role.IsSystem)
                    throw new InvalidOperationException("Cannot rename a system role.");

                var duplicate = await _roleRepository.GetByNameAsync(request.Name);
                if (duplicate != null && duplicate.Id != id)
                    throw new InvalidOperationException($"A role with name '{request.Name}' already exists.");

                role.Name = request.Name;
            }

            if (request.Description != null)
                role.Description = request.Description;

            await _roleRepository.UpdateAsync(role);
        }

        // ── Delete ──────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var role = await _roleRepository.GetByIdWithPermissionsAsync(id)
                ?? throw new KeyNotFoundException($"Role {id} not found.");

            if (role.IsSystem)
                throw new InvalidOperationException("Cannot delete a system role.");

            await _roleRepository.DeleteAsync(role);
        }

        // ── Permissions ─────────────────────────────────────────────────────────

        public async Task UpdatePermissionsAsync(Guid id, UpdateRolePermissionsRequest request)
        {
            _ = await _roleRepository.GetByIdWithPermissionsAsync(id)
                ?? throw new KeyNotFoundException($"Role {id} not found.");

            await _roleRepository.SetRolePermissionsAsync(id, request.PermissionIds);
        }

        public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync()
        {
            var permissions = await _roleRepository.GetAllPermissionsAsync();
            return permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Resource = p.Resource,
                Action = p.Action
            });
        }

        // ── Mapping ─────────────────────────────────────────────────────────────

        private static RoleDto MapToDto(Role role) => new()
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsSystem = role.IsSystem,
            Permissions = role.RolePermissions?.Select(rp => new PermissionDto
            {
                Id = rp.Permission.Id,
                Name = rp.Permission.Name,
                Resource = rp.Permission.Resource,
                Action = rp.Permission.Action
            }).ToList() ?? new()
        };
    }
}
