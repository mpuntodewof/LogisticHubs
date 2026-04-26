using Asp.Versioning;
using API.Filters;
using Domain.Constants;
using Application.DTOs.Common;
using Application.DTOs.Roles;
using Application.UseCases.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/roles")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly RoleUseCase _roleUseCase;

        public RolesController(RoleUseCase roleUseCase)
        {
            _roleUseCase = roleUseCase;
        }

        /// <summary>Get all roles with their permissions (paginated).</summary>
        [HttpGet]
        [RequirePermission(Permissions.Roles.Read)]
        public async Task<ActionResult<PagedResult<RoleDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _roleUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a specific role by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission(Permissions.Roles.Read)]
        public async Task<ActionResult<RoleDto>> GetById(Guid id)
        {
            var role = await _roleUseCase.GetByIdAsync(id);
            if (role == null) return NotFound();
            return Ok(role);
        }

        /// <summary>Create a new custom role.</summary>
        [HttpPost]
        [RequirePermission(Permissions.Roles.Create)]
        public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleRequest request)
        {
            try
            {
                var role = await _roleUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a role's name or description.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission(Permissions.Roles.Update)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                await _roleUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Delete a custom role. System roles cannot be deleted.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission(Permissions.Roles.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _roleUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Replace all permissions for a role.</summary>
        [HttpPut("{id:guid}/permissions")]
        [RequirePermission(Permissions.Roles.Update)]
        public async Task<IActionResult> UpdatePermissions(Guid id, [FromBody] UpdateRolePermissionsRequest request)
        {
            try
            {
                await _roleUseCase.UpdatePermissionsAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Get all available permissions.</summary>
        [HttpGet("/api/permissions")]
        [RequirePermission(Permissions.Roles.Read)]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetAllPermissions()
        {
            var permissions = await _roleUseCase.GetAllPermissionsAsync();
            return Ok(permissions);
        }
    }
}
