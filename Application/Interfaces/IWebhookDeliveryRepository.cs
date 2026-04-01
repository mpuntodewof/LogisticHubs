using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IWebhookDeliveryRepository
    {
        Task<PagedResult<WebhookDelivery>> GetPagedAsync(PagedRequest request, Guid? subscriptionId = null, string? status = null);
        Task<WebhookDelivery?> GetByIdAsync(Guid id);
        Task<WebhookDelivery> CreateAsync(WebhookDelivery delivery);
        Task UpdateAsync(WebhookDelivery delivery);
    }
}
