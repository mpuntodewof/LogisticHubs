using Application.DTOs.Api;
using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Api
{
    public class WebhookDeliveryUseCase
    {
        private readonly IWebhookDeliveryRepository _deliveryRepository;

        public WebhookDeliveryUseCase(IWebhookDeliveryRepository deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
        }

        public async Task<PagedResult<WebhookDeliveryDto>> GetPagedAsync(
            PagedRequest request, Guid? subscriptionId = null, string? status = null)
        {
            var result = await _deliveryRepository.GetPagedAsync(request, subscriptionId, status);

            return new PagedResult<WebhookDeliveryDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<WebhookDeliveryDetailDto?> GetByIdAsync(Guid id)
        {
            var delivery = await _deliveryRepository.GetByIdAsync(id);
            return delivery == null ? null : MapToDetailDto(delivery);
        }

        public async Task RetryAsync(Guid id)
        {
            var delivery = await _deliveryRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Webhook delivery not found.");

            if (delivery.Status != "Failed")
                throw new InvalidOperationException("Only failed deliveries can be retried.");

            delivery.Status = "Pending";
            delivery.RetryCount += 1;
            delivery.ErrorMessage = null;
            delivery.UpdatedAt = DateTime.UtcNow;

            await _deliveryRepository.UpdateAsync(delivery);
        }

        private static WebhookDeliveryDto MapToDto(WebhookDelivery d) => new()
        {
            Id = d.Id,
            WebhookSubscriptionId = d.WebhookSubscriptionId,
            EntityType = d.EntityType,
            EventType = d.EventType,
            EntityId = d.EntityId,
            Status = d.Status,
            ResponseStatusCode = d.ResponseStatusCode,
            RetryCount = d.RetryCount,
            DurationMs = d.DurationMs,
            CreatedAt = d.CreatedAt
        };

        private static WebhookDeliveryDetailDto MapToDetailDto(WebhookDelivery d) => new()
        {
            Id = d.Id,
            WebhookSubscriptionId = d.WebhookSubscriptionId,
            EntityType = d.EntityType,
            EventType = d.EventType,
            EntityId = d.EntityId,
            Status = d.Status,
            ResponseStatusCode = d.ResponseStatusCode,
            RetryCount = d.RetryCount,
            DurationMs = d.DurationMs,
            CreatedAt = d.CreatedAt,
            ResponseBody = d.ResponseBody,
            PayloadJson = d.PayloadJson,
            ErrorMessage = d.ErrorMessage,
            NextRetryAt = d.NextRetryAt
        };
    }
}
