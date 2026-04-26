using Asp.Versioning;
using API.Filters;
using Domain.Constants;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.UseCases.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserUseCase _userUseCase;

        public UsersController(UserUseCase userUseCase)
        {
            _userUseCase = userUseCase;
        }

        /// <summary>Get all users (paginated).</summary>
        [HttpGet]
        [RequirePermission(Permissions.Users.Read)]
        public async Task<ActionResult<PagedResult<UserDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _userUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a specific user by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission(Permissions.Users.Read)]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            var user = await _userUseCase.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>Update a user's name or active status.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission(Permissions.Users.Update)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                await _userUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Delete a user.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission(Permissions.Users.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _userUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Assign a role to a user.</summary>
        [HttpPost("{id:guid}/roles")]
        [RequirePermission(Permissions.Roles.Assign)]
        public async Task<IActionResult> AssignRole(Guid id, [FromBody] AssignRoleRequest request)
        {
            var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");
            Guid.TryParse(currentUserIdClaim, out var assignedBy);

            try
            {
                await _userUseCase.AssignRoleAsync(id, request.RoleId, assignedBy);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Remove a role from a user.</summary>
        [HttpDelete("{id:guid}/roles/{roleId:guid}")]
        [RequirePermission(Permissions.Roles.Assign)]
        public async Task<IActionResult> RevokeRole(Guid id, Guid roleId)
        {
            try
            {
                await _userUseCase.RevokeRoleAsync(id, roleId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
