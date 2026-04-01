using Application.DTOs.Common;
using Application.DTOs.Reporting;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Reporting
{
    public class ReportExecutionUseCase
    {
        private readonly IReportExecutionRepository _executionRepository;
        private readonly IReportDefinitionRepository _definitionRepository;

        public ReportExecutionUseCase(IReportExecutionRepository executionRepository, IReportDefinitionRepository definitionRepository)
        {
            _executionRepository = executionRepository;
            _definitionRepository = definitionRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<ReportExecutionDto>> GetPagedAsync(PagedRequest request, Guid? reportDefinitionId = null, string? status = null)
        {
            var paged = await _executionRepository.GetPagedAsync(request, reportDefinitionId, status);
            return new PagedResult<ReportExecutionDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<ReportExecutionDetailDto?> GetByIdAsync(Guid id)
        {
            var entity = await _executionRepository.GetByIdAsync(id);
            return entity == null ? null : MapToDetailDto(entity);
        }

        // ── Trigger ──────────────────────────────────────────────────────────────

        public async Task<ReportExecutionDto> TriggerAsync(Guid reportDefinitionId, TriggerReportExecutionRequest request)
        {
            var definition = await _definitionRepository.GetByIdAsync(reportDefinitionId)
                ?? throw new KeyNotFoundException($"ReportDefinition {reportDefinitionId} not found.");

            var execution = new ReportExecution
            {
                Id = Guid.NewGuid(),
                ReportDefinitionId = definition.Id,
                Status = "Queued",
                OutputFormat = request.OutputFormat,
                ParametersUsedJson = definition.ParametersJson,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _executionRepository.CreateAsync(execution);
            created.ReportDefinition = definition;
            return MapToDto(created);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static ReportExecutionDto MapToDto(ReportExecution e) => new()
        {
            Id = e.Id,
            ReportDefinitionId = e.ReportDefinitionId,
            ReportName = e.ReportDefinition?.Name ?? string.Empty,
            Status = e.Status,
            StartedAt = e.StartedAt,
            CompletedAt = e.CompletedAt,
            DurationMs = e.DurationMs,
            OutputFileUrl = e.OutputFileUrl,
            OutputFormat = e.OutputFormat,
            TriggeredBy = e.TriggeredBy,
            CreatedAt = e.CreatedAt
        };

        private static ReportExecutionDetailDto MapToDetailDto(ReportExecution e) => new()
        {
            Id = e.Id,
            ReportDefinitionId = e.ReportDefinitionId,
            ReportName = e.ReportDefinition?.Name ?? string.Empty,
            Status = e.Status,
            StartedAt = e.StartedAt,
            CompletedAt = e.CompletedAt,
            DurationMs = e.DurationMs,
            OutputFileUrl = e.OutputFileUrl,
            OutputFormat = e.OutputFormat,
            TriggeredBy = e.TriggeredBy,
            CreatedAt = e.CreatedAt,
            ErrorMessage = e.ErrorMessage,
            ParametersUsedJson = e.ParametersUsedJson
        };
    }
}
