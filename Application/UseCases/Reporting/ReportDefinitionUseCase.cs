using Application.DTOs.Common;
using Application.DTOs.Reporting;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Reporting
{
    public class ReportDefinitionUseCase
    {
        private readonly IReportDefinitionRepository _repository;

        public ReportDefinitionUseCase(IReportDefinitionRepository repository)
        {
            _repository = repository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<ReportDefinitionDto>> GetPagedAsync(PagedRequest request, string? reportType = null, bool? isActive = null)
        {
            var paged = await _repository.GetPagedAsync(request, reportType, isActive);
            return new PagedResult<ReportDefinitionDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<ReportDefinitionDetailDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : MapToDetailDto(entity);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<ReportDefinitionDto> CreateAsync(CreateReportDefinitionRequest request)
        {
            var entity = new ReportDefinition
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                ReportType = request.ReportType.ToString(),
                ParametersJson = request.ParametersJson,
                ScheduleFrequency = request.ScheduleFrequency.ToString(),
                ScheduleTime = request.ScheduleTime,
                ScheduleDayOfWeek = request.ScheduleDayOfWeek,
                ScheduleDayOfMonth = request.ScheduleDayOfMonth,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(entity);
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateReportDefinitionRequest request)
        {
            var entity = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"ReportDefinition {id} not found.");

            if (request.Name != null) entity.Name = request.Name;
            if (request.Description != null) entity.Description = request.Description;
            if (request.ReportType.HasValue) entity.ReportType = request.ReportType.Value.ToString();
            if (request.ParametersJson != null) entity.ParametersJson = request.ParametersJson;
            if (request.ScheduleFrequency.HasValue) entity.ScheduleFrequency = request.ScheduleFrequency.Value.ToString();
            if (request.ScheduleTime != null) entity.ScheduleTime = request.ScheduleTime;
            if (request.ScheduleDayOfWeek.HasValue) entity.ScheduleDayOfWeek = request.ScheduleDayOfWeek;
            if (request.ScheduleDayOfMonth.HasValue) entity.ScheduleDayOfMonth = request.ScheduleDayOfMonth;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;

            await _repository.UpdateAsync(entity);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"ReportDefinition {id} not found.");

            await _repository.DeleteAsync(entity);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static ReportDefinitionDto MapToDto(ReportDefinition e) => new()
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            ReportType = e.ReportType,
            ScheduleFrequency = e.ScheduleFrequency,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt
        };

        private static ReportDefinitionDetailDto MapToDetailDto(ReportDefinition e) => new()
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            ReportType = e.ReportType,
            ScheduleFrequency = e.ScheduleFrequency,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            ParametersJson = e.ParametersJson,
            ScheduleTime = e.ScheduleTime,
            ScheduleDayOfWeek = e.ScheduleDayOfWeek,
            ScheduleDayOfMonth = e.ScheduleDayOfMonth
        };
    }
}
