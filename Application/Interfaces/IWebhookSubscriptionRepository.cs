using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IWebhookSubscriptionRepository
    {
        Task<PagedResult<WebhookSubscription>> GetPagedAsync(PagedRequest request, string? entityType = null, bool? isActive = null);
        Task<WebhookSubscription?> GetByIdAsync(Guid id);
        Task<List<WebhookSubscription>> GetActiveByEventAsync(string entityType, string eventType);
        Task<WebhookSubscription> CreateAsync(WebhookSubscription subscription);
        Task UpdateAsync(WebhookSubscription subscription);
        Task DeleteAsync(WebhookSubscription subscription);
    }
}
