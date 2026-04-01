using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Reporting;
using Application.UseCases.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/dashboard-widgets")]
    [Authorize]
    public class DashboardWidgetsController : ControllerBase
    {
        private readonly DashboardWidgetUseCase _dashboardWidgetUseCase;

        public DashboardWidgetsController(DashboardWidgetUseCase dashboardWidgetUseCase)
        {
            _dashboardWidgetUseCase = dashboardWidgetUseCase;
        }

        /// <summary>Get all dashboard widgets (paginated).</summary>
        [HttpGet]
        [RequirePermission("dashboard-widgets.read")]
        public async Task<ActionResult<PagedResult<DashboardWidgetDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] Guid? userId = null)
        {
            var result = await _dashboardWidgetUseCase.GetPagedAsync(request, userId);
            return Ok(result);
        }

        /// <summary>Get my dashboard widgets.</summary>
        [HttpGet("my")]
        [RequirePermission("dashboard-widgets.read")]
        public async Task<ActionResult<List<DashboardWidgetDto>>> GetMyWidgets([FromQuery] Guid? userId = null)
        {
            var result = await _dashboardWidgetUseCase.GetMyWidgetsAsync(userId);
            return Ok(result);
        }

        /// <summary>Get a dashboard widget by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("dashboard-widgets.read")]
        public async Task<ActionResult<DashboardWidgetDetailDto>> GetById(Guid id)
        {
            var result = await _dashboardWidgetUseCase.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>Create a new dashboard widget.</summary>
        [HttpPost]
        [RequirePermission("dashboard-widgets.create")]
        public async Task<ActionResult<DashboardWidgetDto>> Create([FromBody] CreateDashboardWidgetRequest request)
        {
            var result = await _dashboardWidgetUseCase.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Update a dashboard widget.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("dashboard-widgets.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDashboardWidgetRequest request)
        {
            try
            {
                await _dashboardWidgetUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Delete a dashboard widget.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("dashboard-widgets.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _dashboardWidgetUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
