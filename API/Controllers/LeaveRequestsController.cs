using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.HRM;
using Application.UseCases.HRM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/leave-requests")]
    [Authorize]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly LeaveRequestUseCase _leaveRequestUseCase;

        public LeaveRequestsController(LeaveRequestUseCase leaveRequestUseCase)
        {
            _leaveRequestUseCase = leaveRequestUseCase;
        }

        /// <summary>Get leave requests (paginated).</summary>
        [HttpGet]
        [RequirePermission("leave-requests.read")]
        public async Task<ActionResult<PagedResult<LeaveRequestDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] Guid? employeeId = null,
            [FromQuery] string? status = null)
        {
            var result = await _leaveRequestUseCase.GetPagedAsync(request, employeeId, status);
            return Ok(result);
        }

        /// <summary>Get a leave request by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("leave-requests.read")]
        public async Task<ActionResult<LeaveRequestDto>> GetById(Guid id)
        {
            var leaveRequest = await _leaveRequestUseCase.GetByIdAsync(id);
            if (leaveRequest == null) return NotFound();
            return Ok(leaveRequest);
        }

        /// <summary>Create a new leave request.</summary>
        [HttpPost]
        [RequirePermission("leave-requests.create")]
        public async Task<ActionResult<LeaveRequestDto>> Create([FromBody] CreateLeaveRequestRequest request)
        {
            try
            {
                var leaveRequest = await _leaveRequestUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = leaveRequest.Id }, leaveRequest);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Approve a leave request.</summary>
        [HttpPost("{id:guid}/approve")]
        [RequirePermission("leave-requests.approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            try
            {
                await _leaveRequestUseCase.ApproveAsync(id);
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

        /// <summary>Reject a leave request.</summary>
        [HttpPost("{id:guid}/reject")]
        [RequirePermission("leave-requests.approve")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] RejectLeaveRequestRequest request)
        {
            try
            {
                await _leaveRequestUseCase.RejectAsync(id, request.RejectionReason);
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
    }
}
