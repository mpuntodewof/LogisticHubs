using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPromotionRepository
    {
        Task<PagedResult<Promotion>> GetPagedAsync(PagedRequest request, string? status = null, string? type = null);
        Task<Promotion?> GetByIdAsync(Guid id);
        Task<Promotion?> GetDetailByIdAsync(Guid id);
        Task<IEnumerable<Promotion>> GetActivePromotionsAsync();
        Task<Promotion?> GetByCodeAsync(string code);
        Task<bool> CodeExistsAsync(string code);
        Task<Promotion> CreateAsync(Promotion promotion);
        Task UpdateAsync(Promotion promotion);
        Task DeleteAsync(Promotion promotion);

        Task<PromotionRule> AddRuleAsync(PromotionRule rule);
        Task RemoveRuleAsync(PromotionRule rule);
        Task<PromotionRule?> GetRuleByIdAsync(Guid id);

        Task<PromotionProduct> AddProductAsync(PromotionProduct product);
        Task RemoveProductAsync(PromotionProduct product);
        Task<PromotionProduct?> GetProductByIdAsync(Guid id);

        Task<PromotionUsage> AddUsageAsync(PromotionUsage usage);
        Task<int> CountUsageByCustomerAsync(Guid customerId, Guid promotionId);
    }
}
