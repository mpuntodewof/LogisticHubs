using Application.DTOs.Audit;
using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Audit
{
    public class SystemLogUseCase
    {
        private readonly ISystemLogRepository _repository;

        public SystemLogUseCase(ISystemLogRepository repository)
        {
            _repository = repository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<SystemLogDto>> GetPagedAsync(PagedRequest request, string? level = null, string? source = null, Guid? tenantId = null, DateTime? from = null, DateTime? to = null)
        {
            var paged = await _repository.GetPagedAsync(request, level, source, tenantId, from, to);
            return new PagedResult<SystemLogDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<SystemLogDetailDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : MapToDetailDto(entity);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static SystemLogDto MapToDto(SystemLog e) => new()
        {
            Id = e.Id,
            Level = e.Level,
            Message = e.Message,
            Source = e.Source,
            RequestPath = e.RequestPath,
            TenantId = e.TenantId,
            UserId = e.UserId,
            Timestamp = e.Timestamp
        };

        private static SystemLogDetailDto MapToDetailDto(SystemLog e) => new()
        {
            Id = e.Id,
            Level = e.Level,
            Message = e.Message,
            Source = e.Source,
            RequestPath = e.RequestPath,
            TenantId = e.TenantId,
            UserId = e.UserId,
            Timestamp = e.Timestamp,
            Exception = e.Exception,
            RequestMethod = e.RequestMethod,
            AdditionalDataJson = e.AdditionalDataJson
        };
    }
}
