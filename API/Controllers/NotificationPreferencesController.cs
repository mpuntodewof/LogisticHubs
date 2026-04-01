using API.Filters;
using Application.DTOs.Notifications;
using Application.UseCases.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/notification-preferences")]
    [Authorize]
    public class NotificationPreferencesController : ControllerBase
    {
        private readonly NotificationPreferenceUseCase _preferenceUseCase;

        public NotificationPreferencesController(NotificationPreferenceUseCase preferenceUseCase)
        {
            _preferenceUseCase = preferenceUseCase;
        }

        /// <summary>Get my notification preferences.</summary>
        [HttpGet("my")]
        [RequirePermission("notification-preferences.read")]
        public async Task<ActionResult<NotificationPreferenceDto>> GetMyPreferences()
        {
            var result = await _preferenceUseCase.GetMyPreferencesAsync();
            return Ok(result);
        }

        /// <summary>Update my notification preferences.</summary>
        [HttpPut("my")]
        [RequirePermission("notification-preferences.update")]
        public async Task<ActionResult<NotificationPreferenceDto>> UpdateMyPreferences([FromBody] UpdateNotificationPreferenceRequest request)
        {
            var result = await _preferenceUseCase.UpdateMyPreferencesAsync(request);
            return Ok(result);
        }
    }
}
