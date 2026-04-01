using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.HRM;
using Application.UseCases.HRM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceUseCase _attendanceUseCase;

        public AttendanceController(AttendanceUseCase attendanceUseCase)
        {
            _attendanceUseCase = attendanceUseCase;
        }

        /// <summary>Get attendance records (paginated).</summary>
        [HttpGet]
        [RequirePermission("attendance.read")]
        public async Task<ActionResult<PagedResult<AttendanceDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] Guid? employeeId = null,
            [FromQuery] DateOnly? date = null)
        {
            var result = await _attendanceUseCase.GetPagedAsync(request, employeeId, date);
            return Ok(result);
        }

        /// <summary>Clock in.</summary>
        [HttpPost("clock-in")]
        [RequirePermission("attendance.manage")]
        public async Task<ActionResult<AttendanceDto>> ClockIn([FromBody] ClockInRequest request)
        {
            try
            {
                var attendance = await _attendanceUseCase.ClockInAsync(request);
                return Ok(attendance);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Clock out.</summary>
        [HttpPost("{id:guid}/clock-out")]
        [RequirePermission("attendance.manage")]
        public async Task<ActionResult<AttendanceDto>> ClockOut(Guid id)
        {
            try
            {
                var attendance = await _attendanceUseCase.ClockOutAsync(id);
                return Ok(attendance);
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
