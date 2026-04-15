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
    [Route("api/v{version:apiVersion}/audit-logs")]
    [Authorize]
    public class AuditLogsController : ControllerBase
    {
        private readonly AuditLogUseCase _auditLogUseCase;

        public AuditLogsController(AuditLogUseCase auditLogUseCase)
        {
            _auditLogUseCase = auditLogUseCase;
        }

        /// <summary>Get all audit logs (paginated).</summary>
        [HttpGet]
        [RequirePermission("audit-logs.read")]
        public async Task<ActionResult<PagedResult<AuditLogDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] Guid? userId = null,
            [FromQuery] string? entityType = null,
            [FromQuery] string? action = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var result = await _auditLogUseCase.GetPagedAsync(request, userId, entityType, action, from, to);
            return Ok(result);
        }

        /// <summary>Get an audit log by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("audit-logs.read")]
        public async Task<ActionResult<AuditLogDetailDto>> GetById(Guid id)
        {
            var result = await _auditLogUseCase.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
