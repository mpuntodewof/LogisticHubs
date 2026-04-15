using Asp.Versioning;
using API.Filters;
using Application.DTOs.Export;
using Application.UseCases.Export;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/export")]
    [Authorize]
    public class ExportController : ControllerBase
    {
        private readonly ExportUseCase _exportUseCase;

        public ExportController(ExportUseCase exportUseCase)
        {
            _exportUseCase = exportUseCase;
        }

        [HttpGet("{entityType}")]
        [RequirePermission("audit-logs.export")]
        public async Task<IActionResult> Export(
            string entityType,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string? status)
        {
            var request = new ExportRequest
            {
                EntityType = entityType,
                FromDate = from,
                ToDate = to,
                Status = status
            };

            var (content, fileName) = await _exportUseCase.ExportAsync(request);
            return File(content, "text/csv", fileName);
        }
    }
}
