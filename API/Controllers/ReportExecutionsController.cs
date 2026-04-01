using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Reporting;
using Application.UseCases.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/report-executions")]
    [Authorize]
    public class ReportExecutionsController : ControllerBase
    {
        private readonly ReportExecutionUseCase _reportExecutionUseCase;

        public ReportExecutionsController(ReportExecutionUseCase reportExecutionUseCase)
        {
            _reportExecutionUseCase = reportExecutionUseCase;
        }

        /// <summary>Get all report executions (paginated).</summary>
        [HttpGet]
        [RequirePermission("report-executions.read")]
        public async Task<ActionResult<PagedResult<ReportExecutionDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] Guid? reportDefinitionId = null,
            [FromQuery] string? status = null)
        {
            var result = await _reportExecutionUseCase.GetPagedAsync(request, reportDefinitionId, status);
            return Ok(result);
        }

        /// <summary>Get a report execution by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("report-executions.read")]
        public async Task<ActionResult<ReportExecutionDetailDto>> GetById(Guid id)
        {
            var result = await _reportExecutionUseCase.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
