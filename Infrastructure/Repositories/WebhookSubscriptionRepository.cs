using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WebhookSubscriptionRepository : IWebhookSubscriptionRepository
    {
        private readonly AppDbContext _context;

        public WebhookSubscriptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<WebhookSubscription>> GetPagedAsync(
            PagedRequest request, string? entityType = null, bool? isActive = null)
        {
            var query = _context.WebhookSubscriptions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(entityType))
            {
                query = query.Where(ws => ws.EntityType == entityType);
            }

            if (isActive.HasValue)
            {
                query = query.Where(ws => ws.IsActive == isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(ws => ws.Name.ToLower().Contains(search)
                    || ws.EntityType.ToLower().Contains(search)
                    || ws.CallbackUrl.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(ws => ws.Name) : query.OrderBy(ws => ws.Name),
                "entitytype" => request.SortDescending ? query.OrderByDescending(ws => ws.EntityType) : query.OrderBy(ws => ws.EntityType),
                _ => query.OrderBy(ws => ws.Name)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<WebhookSubscription?> GetByIdAsync(Guid id)
            => await _context.WebhookSubscriptions.FirstOrDefaultAsync(ws => ws.Id == id);

        public async Task<List<WebhookSubscription>> GetActiveByEventAsync(string entityType, string eventType)
            => await _context.WebhookSubscriptions
                .Where(ws => ws.IsActive
                    && (ws.EntityType == entityType || ws.EntityType == "*")
                    && (ws.EventType == eventType || ws.EventType == "*"))
                .ToListAsync();

        public async Task<WebhookSubscription> CreateAsync(WebhookSubscription subscription)
        {
            _context.WebhookSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task UpdateAsync(WebhookSubscription subscription)
        {
            _context.WebhookSubscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(WebhookSubscription subscription)
        {
            _context.WebhookSubscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
        }
    }
}
