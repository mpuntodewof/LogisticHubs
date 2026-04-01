using Application.DTOs.Api;
using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Api
{
    public class WebhookSubscriptionUseCase
    {
        private readonly IWebhookSubscriptionRepository _subscriptionRepository;
        private readonly IWebhookDeliveryRepository _deliveryRepository;

        public WebhookSubscriptionUseCase(
            IWebhookSubscriptionRepository subscriptionRepository,
            IWebhookDeliveryRepository deliveryRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _deliveryRepository = deliveryRepository;
        }

        public async Task<PagedResult<WebhookSubscriptionDto>> GetPagedAsync(
            PagedRequest request, string? entityType = null, bool? isActive = null)
        {
            var result = await _subscriptionRepository.GetPagedAsync(request, entityType, isActive);

            return new PagedResult<WebhookSubscriptionDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<WebhookSubscriptionDetailDto?> GetByIdAsync(Guid id)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(id);
            return subscription == null ? null : MapToDetailDto(subscription);
        }

        public async Task<WebhookSubscriptionDto> CreateAsync(CreateWebhookSubscriptionRequest request)
        {
            var subscription = new WebhookSubscription
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                EntityType = request.EntityType,
                EventType = request.EventType,
                CallbackUrl = request.CallbackUrl,
                Secret = request.Secret,
                HeadersJson = request.HeadersJson,
                MaxRetries = request.MaxRetries ?? 3,
                TimeoutSeconds = request.TimeoutSeconds ?? 30,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _subscriptionRepository.CreateAsync(subscription);
            return MapToDto(created);
        }

        public async Task UpdateAsync(Guid id, UpdateWebhookSubscriptionRequest request)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Webhook subscription not found.");

            if (request.Name != null) subscription.Name = request.Name;
            if (request.EntityType != null) subscription.EntityType = request.EntityType;
            if (request.EventType != null) subscription.EventType = request.EventType;
            if (request.CallbackUrl != null) subscription.CallbackUrl = request.CallbackUrl;
            if (request.Secret != null) subscription.Secret = request.Secret;
            if (request.HeadersJson != null) subscription.HeadersJson = request.HeadersJson;
            if (request.MaxRetries.HasValue) subscription.MaxRetries = request.MaxRetries.Value;
            if (request.TimeoutSeconds.HasValue) subscription.TimeoutSeconds = request.TimeoutSeconds.Value;
            if (request.IsActive.HasValue) subscription.IsActive = request.IsActive.Value;

            subscription.UpdatedAt = DateTime.UtcNow;

            await _subscriptionRepository.UpdateAsync(subscription);
        }

        public async Task DeleteAsync(Guid id)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Webhook subscription not found.");

            await _subscriptionRepository.DeleteAsync(subscription);
        }

        public async Task<WebhookDeliveryDto> TestAsync(Guid id)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Webhook subscription not found.");

            var delivery = new WebhookDelivery
            {
                Id = Guid.NewGuid(),
                WebhookSubscriptionId = subscription.Id,
                EntityType = "test",
                EventType = "test",
                Status = "Pending",
                PayloadJson = "{\"test\":true}",
                RetryCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _deliveryRepository.CreateAsync(delivery);

            return new WebhookDeliveryDto
            {
                Id = created.Id,
                WebhookSubscriptionId = created.WebhookSubscriptionId,
                EntityType = created.EntityType,
                EventType = created.EventType,
                EntityId = created.EntityId,
                Status = created.Status,
                ResponseStatusCode = created.ResponseStatusCode,
                RetryCount = created.RetryCount,
                DurationMs = created.DurationMs,
                CreatedAt = created.CreatedAt
            };
        }

        private static WebhookSubscriptionDto MapToDto(WebhookSubscription ws) => new()
        {
            Id = ws.Id,
            Name = ws.Name,
            EntityType = ws.EntityType,
            EventType = ws.EventType,
            CallbackUrl = ws.CallbackUrl,
            IsActive = ws.IsActive,
            MaxRetries = ws.MaxRetries,
            TimeoutSeconds = ws.TimeoutSeconds,
            CreatedAt = ws.CreatedAt
        };

        private static WebhookSubscriptionDetailDto MapToDetailDto(WebhookSubscription ws) => new()
        {
            Id = ws.Id,
            Name = ws.Name,
            EntityType = ws.EntityType,
            EventType = ws.EventType,
            CallbackUrl = ws.CallbackUrl,
            IsActive = ws.IsActive,
            MaxRetries = ws.MaxRetries,
            TimeoutSeconds = ws.TimeoutSeconds,
            CreatedAt = ws.CreatedAt,
            HeadersJson = ws.HeadersJson,
            Secret = MaskSecret(ws.Secret)
        };

        private static string? MaskSecret(string? secret)
        {
            if (string.IsNullOrEmpty(secret)) return null;
            if (secret.Length <= 4) return "****";
            return "****" + secret[^4..];
        }
    }
}
