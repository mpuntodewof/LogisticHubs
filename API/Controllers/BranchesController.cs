using API.Filters;
using Application.DTOs.Branches;
using Application.DTOs.Common;
using Application.UseCases.Branches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/branches")]
    [Authorize]
    public class BranchesController : ControllerBase
    {
        private readonly BranchUseCase _branchUseCase;

        public BranchesController(BranchUseCase branchUseCase)
        {
            _branchUseCase = branchUseCase;
        }

        /// <summary>Get all branches (paginated).</summary>
        [HttpGet]
        [RequirePermission("branches.read")]
        public async Task<ActionResult<PagedResult<BranchDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _branchUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a branch by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("branches.read")]
        public async Task<ActionResult<BranchDto>> GetById(Guid id)
        {
            var branch = await _branchUseCase.GetByIdAsync(id);
            if (branch == null) return NotFound();
            return Ok(branch);
        }

        /// <summary>Create a new branch.</summary>
        [HttpPost]
        [RequirePermission("branches.create")]
        public async Task<ActionResult<BranchDto>> Create([FromBody] CreateBranchRequest request)
        {
            try
            {
                var branch = await _branchUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = branch.Id }, branch);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a branch.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("branches.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBranchRequest request)
        {
            try
            {
                await _branchUseCase.UpdateAsync(id, request);
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

        /// <summary>Delete a branch.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("branches.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _branchUseCase.DeleteAsync(id);
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

        // ── Branch User Endpoints ────────────────────────────────────────────────

        /// <summary>Get all users assigned to a branch.</summary>
        [HttpGet("{id:guid}/users")]
        [RequirePermission("branches.read")]
        public async Task<ActionResult<IEnumerable<BranchUserDto>>> GetBranchUsers(Guid id)
        {
            var users = await _branchUseCase.GetBranchUsersAsync(id);
            return Ok(users);
        }

        /// <summary>Assign a user to a branch.</summary>
        [HttpPost("{id:guid}/users")]
        [RequirePermission("branches.assign")]
        public async Task<IActionResult> AssignUser(Guid id, [FromBody] AssignBranchUserRequest request)
        {
            try
            {
                await _branchUseCase.AssignUserAsync(id, request);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Remove a user from a branch.</summary>
        [HttpDelete("{id:guid}/users/{userId:guid}")]
        [RequirePermission("branches.assign")]
        public async Task<IActionResult> RemoveUser(Guid id, Guid userId)
        {
            await _branchUseCase.RemoveUserAsync(id, userId);
            return NoContent();
        }
    }
}
