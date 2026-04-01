using Application.DTOs.Common;
using Application.DTOs.Notifications;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Notifications
{
    public class NotificationTemplateUseCase
    {
        private readonly INotificationTemplateRepository _templateRepository;

        public NotificationTemplateUseCase(INotificationTemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<NotificationTemplateDto>> GetPagedAsync(PagedRequest request)
        {
            var paged = await _templateRepository.GetPagedAsync(request);
            return new PagedResult<NotificationTemplateDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<NotificationTemplateDto?> GetByIdAsync(Guid id)
        {
            var template = await _templateRepository.GetByIdAsync(id);
            return template == null ? null : MapToDto(template);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<NotificationTemplateDto> CreateAsync(CreateNotificationTemplateRequest request)
        {
            if (await _templateRepository.CodeExistsAsync(request.Code))
                throw new InvalidOperationException($"A notification template with code '{request.Code}' already exists.");

            var template = new NotificationTemplate
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                Name = request.Name,
                Channel = request.Channel.ToString(),
                Subject = request.Subject,
                BodyTemplate = request.BodyTemplate,
                Description = request.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _templateRepository.CreateAsync(template);
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateNotificationTemplateRequest request)
        {
            var template = await _templateRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Notification template {id} not found.");

            if (request.Name != null) template.Name = request.Name;
            if (request.Subject != null) template.Subject = request.Subject;
            if (request.BodyTemplate != null) template.BodyTemplate = request.BodyTemplate;
            if (request.Description != null) template.Description = request.Description;
            if (request.IsActive.HasValue) template.IsActive = request.IsActive.Value;

            await _templateRepository.UpdateAsync(template);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var template = await _templateRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Notification template {id} not found.");

            await _templateRepository.DeleteAsync(template);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static NotificationTemplateDto MapToDto(NotificationTemplate t) => new()
        {
            Id = t.Id,
            Code = t.Code,
            Name = t.Name,
            Channel = t.Channel,
            Subject = t.Subject,
            BodyTemplate = t.BodyTemplate,
            Description = t.Description,
            IsActive = t.IsActive,
            CreatedAt = t.CreatedAt
        };
    }
}
