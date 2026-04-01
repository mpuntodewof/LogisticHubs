using Application.DTOs.Common;
using Application.DTOs.Notifications;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Notifications
{
    public class NotificationUseCase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUserService;

        public NotificationUseCase(INotificationRepository notificationRepository, ICurrentUserService currentUserService)
        {
            _notificationRepository = notificationRepository;
            _currentUserService = currentUserService;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<NotificationDto>> GetMyNotificationsAsync(PagedRequest request, string? status)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var paged = await _notificationRepository.GetPagedByUserAsync(userId, request, status);
            return new PagedResult<NotificationDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<int> GetUnreadCountAsync()
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            return await _notificationRepository.GetUnreadCountAsync(userId);
        }

        public async Task<NotificationDto?> GetByIdAsync(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            return notification == null ? null : MapToDto(notification);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<NotificationDto> CreateAsync(CreateNotificationRequest request)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Title = request.Title,
                Message = request.Message,
                Channel = request.Channel.ToString(),
                Status = "Unread",
                SourceEntityType = request.SourceEntityType,
                SourceEntityId = request.SourceEntityId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _notificationRepository.CreateAsync(notification);
            return MapToDto(created);
        }

        // ── Mark as Read ─────────────────────────────────────────────────────────

        public async Task MarkAsReadAsync(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Notification {id} not found.");

            notification.Status = "Read";
            notification.ReadAt = DateTime.UtcNow;

            await _notificationRepository.UpdateAsync(notification);
        }

        public async Task MarkAllReadAsync()
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            await _notificationRepository.MarkAllReadAsync(userId);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Notification {id} not found.");

            await _notificationRepository.DeleteAsync(notification);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static NotificationDto MapToDto(Notification n) => new()
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Channel = n.Channel,
            Status = n.Status,
            SourceEntityType = n.SourceEntityType,
            SourceEntityId = n.SourceEntityId,
            ReadAt = n.ReadAt,
            SentAt = n.SentAt,
            CreatedAt = n.CreatedAt
        };
    }
}
