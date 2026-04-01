using API.Filters;
using Application.DTOs.Api;
using Application.DTOs.Common;
using Application.UseCases.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/webhook-deliveries")]
    [Authorize]
    public class WebhookDeliveriesController : ControllerBase
    {
        private readonly WebhookDeliveryUseCase _webhookDeliveryUseCase;

        public WebhookDeliveriesController(WebhookDeliveryUseCase webhookDeliveryUseCase)
        {
            _webhookDeliveryUseCase = webhookDeliveryUseCase;
        }

        /// <summary>Get all webhook deliveries (paginated).</summary>
        [HttpGet]
        [RequirePermission("webhook-deliveries.read")]
        public async Task<ActionResult<PagedResult<WebhookDeliveryDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] Guid? subscriptionId,
            [FromQuery] string? status)
        {
            var result = await _webhookDeliveryUseCase.GetPagedAsync(request, subscriptionId, status);
            return Ok(result);
        }

        /// <summary>Get a webhook delivery by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("webhook-deliveries.read")]
        public async Task<ActionResult<WebhookDeliveryDetailDto>> GetById(Guid id)
        {
            var delivery = await _webhookDeliveryUseCase.GetByIdAsync(id);
            if (delivery == null) return NotFound();
            return Ok(delivery);
        }

        /// <summary>Retry a failed webhook delivery.</summary>
        [HttpPost("{id:guid}/retry")]
        [RequirePermission("webhook-deliveries.retry")]
        public async Task<IActionResult> Retry(Guid id)
        {
            try
            {
                await _webhookDeliveryUseCase.RetryAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
