using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly AppDbContext _context;

        public PromotionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Promotion>> GetPagedAsync(PagedRequest request, string? status = null, string? type = null)
        {
            var query = _context.Promotions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p => p.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(p => p.PromotionType == type);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(search)
                    || (p.Code != null && p.Code.ToLower().Contains(search)));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "priority" => request.SortDescending ? query.OrderByDescending(p => p.Priority) : query.OrderBy(p => p.Priority),
                "startdate" => request.SortDescending ? query.OrderByDescending(p => p.StartDate) : query.OrderBy(p => p.StartDate),
                _ => query.OrderByDescending(p => p.StartDate)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<Promotion?> GetByIdAsync(Guid id)
            => await _context.Promotions.FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Promotion?> GetDetailByIdAsync(Guid id)
            => await _context.Promotions
                .Include(p => p.Rules).ThenInclude(r => r.CustomerGroup)
                .Include(p => p.Rules).ThenInclude(r => r.Category)
                .Include(p => p.Products).ThenInclude(pp => pp.Product)
                .Include(p => p.Products).ThenInclude(pp => pp.ProductVariant)
                .Include(p => p.Usages)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Promotions
                .Where(p => p.Status == "Active" && p.IsActive && p.StartDate <= now && p.EndDate >= now)
                .OrderBy(p => p.Priority)
                .ToListAsync();
        }

        public async Task<Promotion?> GetByCodeAsync(string code)
            => await _context.Promotions.FirstOrDefaultAsync(p => p.Code == code);

        public async Task<bool> CodeExistsAsync(string code)
            => await _context.Promotions.AnyAsync(p => p.Code == code);

        public async Task<Promotion> CreateAsync(Promotion promotion)
        {
            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();
            return promotion;
        }

        public async Task UpdateAsync(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Promotion promotion)
        {
            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task<PromotionRule> AddRuleAsync(PromotionRule rule)
        {
            _context.PromotionRules.Add(rule);
            await _context.SaveChangesAsync();
            return rule;
        }

        public async Task RemoveRuleAsync(PromotionRule rule)
        {
            _context.PromotionRules.Remove(rule);
            await _context.SaveChangesAsync();
        }

        public async Task<PromotionRule?> GetRuleByIdAsync(Guid id)
            => await _context.PromotionRules
                .Include(r => r.CustomerGroup)
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<PromotionProduct> AddProductAsync(PromotionProduct product)
        {
            _context.PromotionProducts.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task RemoveProductAsync(PromotionProduct product)
        {
            _context.PromotionProducts.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<PromotionProduct?> GetProductByIdAsync(Guid id)
            => await _context.PromotionProducts
                .Include(pp => pp.Product)
                .Include(pp => pp.ProductVariant)
                .FirstOrDefaultAsync(pp => pp.Id == id);

        public async Task<PromotionUsage> AddUsageAsync(PromotionUsage usage)
        {
            _context.PromotionUsages.Add(usage);
            await _context.SaveChangesAsync();
            return usage;
        }

        public async Task<int> CountUsageByCustomerAsync(Guid customerId, Guid promotionId)
            => await _context.PromotionUsages
                .CountAsync(u => u.CustomerId == customerId && u.PromotionId == promotionId);
    }
}
