using API.Filters;
using Application.DTOs.Api;
using Application.DTOs.Common;
using Application.UseCases.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/webhook-subscriptions")]
    [Authorize]
    public class WebhookSubscriptionsController : ControllerBase
    {
        private readonly WebhookSubscriptionUseCase _webhookSubscriptionUseCase;

        public WebhookSubscriptionsController(WebhookSubscriptionUseCase webhookSubscriptionUseCase)
        {
            _webhookSubscriptionUseCase = webhookSubscriptionUseCase;
        }

        /// <summary>Get all webhook subscriptions (paginated).</summary>
        [HttpGet]
        [RequirePermission("webhooks.read")]
        public async Task<ActionResult<PagedResult<WebhookSubscriptionDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] string? entityType,
            [FromQuery] bool? isActive)
        {
            var result = await _webhookSubscriptionUseCase.GetPagedAsync(request, entityType, isActive);
            return Ok(result);
        }

        /// <summary>Get a webhook subscription by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("webhooks.read")]
        public async Task<ActionResult<WebhookSubscriptionDetailDto>> GetById(Guid id)
        {
            var subscription = await _webhookSubscriptionUseCase.GetByIdAsync(id);
            if (subscription == null) return NotFound();
            return Ok(subscription);
        }

        /// <summary>Create a new webhook subscription.</summary>
        [HttpPost]
        [RequirePermission("webhooks.create")]
        public async Task<ActionResult<WebhookSubscriptionDto>> Create([FromBody] CreateWebhookSubscriptionRequest request)
        {
            try
            {
                var subscription = await _webhookSubscriptionUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = subscription.Id }, subscription);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a webhook subscription.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("webhooks.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWebhookSubscriptionRequest request)
        {
            try
            {
                await _webhookSubscriptionUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Delete a webhook subscription.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("webhooks.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _webhookSubscriptionUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Test a webhook subscription by sending a test delivery.</summary>
        [HttpPost("{id:guid}/test")]
        [RequirePermission("webhooks.test")]
        public async Task<ActionResult<WebhookDeliveryDto>> Test(Guid id)
        {
            try
            {
                var delivery = await _webhookSubscriptionUseCase.TestAsync(id);
                return Ok(delivery);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
