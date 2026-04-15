using Asp.Versioning;
using Application.DTOs.Notifications;
using Application.UseCases.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationUseCase _notificationUseCase;

        public NotificationsController(NotificationUseCase notificationUseCase)
        {
            _notificationUseCase = notificationUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<NotificationSummary>> GetNotifications()
        {
            var summary = await _notificationUseCase.GetActiveNotificationsAsync();
            return Ok(summary);
        }
    }
}
