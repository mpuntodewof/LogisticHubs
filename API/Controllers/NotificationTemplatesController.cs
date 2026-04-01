using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Notifications;
using Application.UseCases.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/notification-templates")]
    [Authorize]
    public class NotificationTemplatesController : ControllerBase
    {
        private readonly NotificationTemplateUseCase _templateUseCase;

        public NotificationTemplatesController(NotificationTemplateUseCase templateUseCase)
        {
            _templateUseCase = templateUseCase;
        }

        /// <summary>Get all notification templates (paginated).</summary>
        [HttpGet]
        [RequirePermission("notification-templates.read")]
        public async Task<ActionResult<PagedResult<NotificationTemplateDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _templateUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a notification template by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("notification-templates.read")]
        public async Task<ActionResult<NotificationTemplateDto>> GetById(Guid id)
        {
            var template = await _templateUseCase.GetByIdAsync(id);
            if (template == null) return NotFound();
            return Ok(template);
        }

        /// <summary>Create a new notification template.</summary>
        [HttpPost]
        [RequirePermission("notification-templates.create")]
        public async Task<ActionResult<NotificationTemplateDto>> Create([FromBody] CreateNotificationTemplateRequest request)
        {
            try
            {
                var template = await _templateUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = template.Id }, template);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a notification template.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("notification-templates.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNotificationTemplateRequest request)
        {
            try
            {
                await _templateUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Delete a notification template.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("notification-templates.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _templateUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
