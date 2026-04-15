using Asp.Versioning;
using API.Filters;
using Application.DTOs.Audit;
using Application.DTOs.Common;
using Application.UseCases.Audit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/system-logs")]
    [Authorize]
    public class SystemLogsController : ControllerBase
    {
        private readonly SystemLogUseCase _systemLogUseCase;

        public SystemLogsController(SystemLogUseCase systemLogUseCase)
        {
            _systemLogUseCase = systemLogUseCase;
        }

        /// <summary>Get all system logs (paginated).</summary>
        [HttpGet]
        [RequirePermission("system-logs.read")]
        public async Task<ActionResult<PagedResult<SystemLogDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] string? level = null,
            [FromQuery] string? source = null,
            [FromQuery] Guid? tenantId = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var result = await _systemLogUseCase.GetPagedAsync(request, level, source, tenantId, from, to);
            return Ok(result);
        }

        /// <summary>Get a system log by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("system-logs.read")]
        public async Task<ActionResult<SystemLogDetailDto>> GetById(Guid id)
        {
            var result = await _systemLogUseCase.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
