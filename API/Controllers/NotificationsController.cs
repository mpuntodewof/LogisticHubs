using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Notifications;
using Application.UseCases.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationUseCase _notificationUseCase;

        public NotificationsController(NotificationUseCase notificationUseCase)
        {
            _notificationUseCase = notificationUseCase;
        }

        /// <summary>Get my notifications (paginated).</summary>
        [HttpGet("my")]
        [RequirePermission("notifications.read")]
        public async Task<ActionResult<PagedResult<NotificationDto>>> GetMyNotifications([FromQuery] PagedRequest request, [FromQuery] string? status)
        {
            var result = await _notificationUseCase.GetMyNotificationsAsync(request, status);
            return Ok(result);
        }

        /// <summary>Get my unread notification count.</summary>
        [HttpGet("my/unread-count")]
        [RequirePermission("notifications.read")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var count = await _notificationUseCase.GetUnreadCountAsync();
            return Ok(count);
        }

        /// <summary>Get a notification by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("notifications.read")]
        public async Task<ActionResult<NotificationDto>> GetById(Guid id)
        {
            var notification = await _notificationUseCase.GetByIdAsync(id);
            if (notification == null) return NotFound();
            return Ok(notification);
        }

        /// <summary>Create a new notification.</summary>
        [HttpPost]
        [RequirePermission("notifications.create")]
        public async Task<ActionResult<NotificationDto>> Create([FromBody] CreateNotificationRequest request)
        {
            var notification = await _notificationUseCase.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = notification.Id }, notification);
        }

        /// <summary>Mark a notification as read.</summary>
        [HttpPost("{id:guid}/read")]
        [RequirePermission("notifications.read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            try
            {
                await _notificationUseCase.MarkAsReadAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Mark all my notifications as read.</summary>
        [HttpPost("my/read-all")]
        [RequirePermission("notifications.read")]
        public async Task<IActionResult> MarkAllRead()
        {
            await _notificationUseCase.MarkAllReadAsync();
            return NoContent();
        }

        /// <summary>Delete a notification.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("notifications.manage")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _notificationUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
