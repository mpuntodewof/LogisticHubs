using Application.DTOs.Audit;
using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Audit
{
    public class AuditLogUseCase
    {
        private readonly IAuditLogRepository _repository;

        public AuditLogUseCase(IAuditLogRepository repository)
        {
            _repository = repository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<AuditLogDto>> GetPagedAsync(PagedRequest request, Guid? userId = null, string? entityType = null, string? action = null, DateTime? from = null, DateTime? to = null)
        {
            var paged = await _repository.GetPagedAsync(request, userId, entityType, action, from, to);
            return new PagedResult<AuditLogDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<AuditLogDetailDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : MapToDetailDto(entity);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static AuditLogDto MapToDto(AuditLog e) => new()
        {
            Id = e.Id,
            UserId = e.UserId,
            UserEmail = e.UserEmail,
            Action = e.Action,
            EntityType = e.EntityType,
            EntityId = e.EntityId,
            IpAddress = e.IpAddress,
            Timestamp = e.Timestamp
        };

        private static AuditLogDetailDto MapToDetailDto(AuditLog e) => new()
        {
            Id = e.Id,
            UserId = e.UserId,
            UserEmail = e.UserEmail,
            Action = e.Action,
            EntityType = e.EntityType,
            EntityId = e.EntityId,
            IpAddress = e.IpAddress,
            Timestamp = e.Timestamp,
            OldValuesJson = e.OldValuesJson,
            NewValuesJson = e.NewValuesJson,
            UserAgent = e.UserAgent,
            AdditionalInfo = e.AdditionalInfo
        };
    }
}
