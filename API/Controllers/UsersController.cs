using API.Filters;
using Application.DTOs.Users;
using Application.UseCases.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserUseCase _userUseCase;

        public UsersController(UserUseCase userUseCase)
        {
            _userUseCase = userUseCase;
        }

        /// <summary>Get all users.</summary>
        [HttpGet]
        [RequirePermission("users.read")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userUseCase.GetAllAsync();
            return Ok(users);
        }

        /// <summary>Get a specific user by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("users.read")]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            var user = await _userUseCase.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>Update a user's name or active status.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("users.update")]
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
        [RequirePermission("users.delete")]
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
        [RequirePermission("roles.assign")]
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
        [RequirePermission("roles.assign")]
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
