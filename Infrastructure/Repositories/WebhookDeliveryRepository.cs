using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WebhookDeliveryRepository : IWebhookDeliveryRepository
    {
        private readonly AppDbContext _context;

        public WebhookDeliveryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<WebhookDelivery>> GetPagedAsync(
            PagedRequest request, Guid? subscriptionId = null, string? status = null)
        {
            var query = _context.WebhookDeliveries.AsQueryable();

            if (subscriptionId.HasValue)
            {
                query = query.Where(d => d.WebhookSubscriptionId == subscriptionId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(d => d.Status == status);
            }

            query = query.OrderByDescending(d => d.CreatedAt);

            return await query.ToPagedResultAsync(request);
        }

        public async Task<WebhookDelivery?> GetByIdAsync(Guid id)
            => await _context.WebhookDeliveries
                .Include(d => d.WebhookSubscription)
                .FirstOrDefaultAsync(d => d.Id == id);

        public async Task<WebhookDelivery> CreateAsync(WebhookDelivery delivery)
        {
            _context.WebhookDeliveries.Add(delivery);
            await _context.SaveChangesAsync();
            return delivery;
        }

        public async Task UpdateAsync(WebhookDelivery delivery)
        {
            _context.WebhookDeliveries.Update(delivery);
            await _context.SaveChangesAsync();
        }
    }
}
