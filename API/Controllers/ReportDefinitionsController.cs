using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Reporting;
using Application.UseCases.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/report-definitions")]
    [Authorize]
    public class ReportDefinitionsController : ControllerBase
    {
        private readonly ReportDefinitionUseCase _reportDefinitionUseCase;
        private readonly ReportExecutionUseCase _reportExecutionUseCase;

        public ReportDefinitionsController(ReportDefinitionUseCase reportDefinitionUseCase, ReportExecutionUseCase reportExecutionUseCase)
        {
            _reportDefinitionUseCase = reportDefinitionUseCase;
            _reportExecutionUseCase = reportExecutionUseCase;
        }

        /// <summary>Get all report definitions (paginated).</summary>
        [HttpGet]
        [RequirePermission("reports.read")]
        public async Task<ActionResult<PagedResult<ReportDefinitionDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] string? reportType = null,
            [FromQuery] bool? isActive = null)
        {
            var result = await _reportDefinitionUseCase.GetPagedAsync(request, reportType, isActive);
            return Ok(result);
        }

        /// <summary>Get a report definition by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("reports.read")]
        public async Task<ActionResult<ReportDefinitionDetailDto>> GetById(Guid id)
        {
            var result = await _reportDefinitionUseCase.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>Create a new report definition.</summary>
        [HttpPost]
        [RequirePermission("reports.create")]
        public async Task<ActionResult<ReportDefinitionDto>> Create([FromBody] CreateReportDefinitionRequest request)
        {
            var result = await _reportDefinitionUseCase.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Update a report definition.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("reports.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReportDefinitionRequest request)
        {
            try
            {
                await _reportDefinitionUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Delete a report definition.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("reports.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _reportDefinitionUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Trigger a report execution.</summary>
        [HttpPost("{id:guid}/execute")]
        [RequirePermission("reports.execute")]
        public async Task<ActionResult<ReportExecutionDto>> Execute(Guid id, [FromBody] TriggerReportExecutionRequest request)
        {
            try
            {
                var result = await _reportExecutionUseCase.TriggerAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
