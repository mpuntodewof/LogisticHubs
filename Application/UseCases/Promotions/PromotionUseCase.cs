using Application.DTOs.Common;
using Application.DTOs.Promotions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Promotions
{
    public class PromotionUseCase
    {
        private readonly IPromotionRepository _promotionRepository;

        public PromotionUseCase(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public async Task<PagedResult<PromotionDto>> GetPagedAsync(
            PagedRequest request, string? status = null, string? type = null)
        {
            var result = await _promotionRepository.GetPagedAsync(request, status, type);

            return new PagedResult<PromotionDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<PromotionDetailDto?> GetByIdAsync(Guid id)
        {
            var promotion = await _promotionRepository.GetDetailByIdAsync(id);
            return promotion == null ? null : MapToDetailDto(promotion);
        }

        public async Task<PromotionDto> CreateAsync(CreatePromotionRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                if (await _promotionRepository.CodeExistsAsync(request.Code))
                    throw new InvalidOperationException($"Promotion code '{request.Code}' already exists.");
            }

            var promotion = new Promotion
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                PromotionType = request.PromotionType.ToString(),
                Status = PromotionStatus.Draft.ToString(),
                DiscountType = request.DiscountType?.ToString(),
                DiscountValue = request.DiscountValue,
                MaxDiscountAmount = request.MaxDiscountAmount,
                BuyQuantity = request.BuyQuantity,
                GetQuantity = request.GetQuantity,
                MinOrderAmount = request.MinOrderAmount,
                MaxUsageCount = request.MaxUsageCount,
                MaxUsagePerCustomer = request.MaxUsagePerCustomer,
                CurrentUsageCount = 0,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsStackable = request.IsStackable,
                Priority = request.Priority,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _promotionRepository.CreateAsync(promotion);
            return MapToDto(created);
        }

        public async Task UpdateAsync(Guid id, UpdatePromotionRequest request)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Promotion not found.");

            if (promotion.Status != PromotionStatus.Draft.ToString() && promotion.Status != PromotionStatus.Paused.ToString())
                throw new InvalidOperationException("Only draft or paused promotions can be updated.");

            if (request.Name != null) promotion.Name = request.Name;
            if (request.Description != null) promotion.Description = request.Description;
            if (request.DiscountValue.HasValue) promotion.DiscountValue = request.DiscountValue.Value;
            if (request.MaxDiscountAmount.HasValue) promotion.MaxDiscountAmount = request.MaxDiscountAmount.Value;
            if (request.MinOrderAmount.HasValue) promotion.MinOrderAmount = request.MinOrderAmount.Value;
            if (request.MaxUsageCount.HasValue) promotion.MaxUsageCount = request.MaxUsageCount.Value;
            if (request.MaxUsagePerCustomer.HasValue) promotion.MaxUsagePerCustomer = request.MaxUsagePerCustomer.Value;
            if (request.EndDate.HasValue) promotion.EndDate = request.EndDate.Value;
            if (request.IsStackable.HasValue) promotion.IsStackable = request.IsStackable.Value;
            if (request.Priority.HasValue) promotion.Priority = request.Priority.Value;
            if (request.IsActive.HasValue) promotion.IsActive = request.IsActive.Value;

            promotion.UpdatedAt = DateTime.UtcNow;

            await _promotionRepository.UpdateAsync(promotion);
        }

        public async Task DeleteAsync(Guid id)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Promotion not found.");

            if (promotion.Status != PromotionStatus.Draft.ToString() && promotion.Status != PromotionStatus.Cancelled.ToString())
                throw new InvalidOperationException("Only draft or cancelled promotions can be deleted.");

            await _promotionRepository.DeleteAsync(promotion);
        }

        public async Task ActivateAsync(Guid id)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Promotion not found.");

            if (promotion.Status != PromotionStatus.Draft.ToString() && promotion.Status != PromotionStatus.Paused.ToString())
                throw new InvalidOperationException("Only draft or paused promotions can be activated.");

            promotion.Status = PromotionStatus.Active.ToString();
            promotion.UpdatedAt = DateTime.UtcNow;

            await _promotionRepository.UpdateAsync(promotion);
        }

        public async Task PauseAsync(Guid id)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Promotion not found.");

            if (promotion.Status != PromotionStatus.Active.ToString())
                throw new InvalidOperationException("Only active promotions can be paused.");

            promotion.Status = PromotionStatus.Paused.ToString();
            promotion.UpdatedAt = DateTime.UtcNow;

            await _promotionRepository.UpdateAsync(promotion);
        }

        public async Task CancelAsync(Guid id)
        {
            var promotion = await _promotionRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Promotion not found.");

            promotion.Status = PromotionStatus.Cancelled.ToString();
            promotion.UpdatedAt = DateTime.UtcNow;

            await _promotionRepository.UpdateAsync(promotion);
        }

        public async Task<PromotionRuleDto> AddRuleAsync(Guid promotionId, CreatePromotionRuleRequest request)
        {
            var promotion = await _promotionRepository.GetByIdAsync(promotionId)
                ?? throw new InvalidOperationException("Promotion not found.");

            var rule = new PromotionRule
            {
                Id = Guid.NewGuid(),
                PromotionId = promotionId,
                RuleType = request.RuleType.ToString(),
                MinQuantity = request.MinQuantity,
                MinOrderAmount = request.MinOrderAmount,
                CustomerGroupId = request.CustomerGroupId,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _promotionRepository.AddRuleAsync(rule);

            // Re-fetch with includes for the DTO
            var fetched = await _promotionRepository.GetRuleByIdAsync(created.Id);
            return MapRuleToDto(fetched!);
        }

        public async Task RemoveRuleAsync(Guid promotionId, Guid ruleId)
        {
            var rule = await _promotionRepository.GetRuleByIdAsync(ruleId)
                ?? throw new InvalidOperationException("Promotion rule not found.");

            if (rule.PromotionId != promotionId)
                throw new InvalidOperationException("Rule does not belong to this promotion.");

            await _promotionRepository.RemoveRuleAsync(rule);
        }

        public async Task<PromotionProductDto> AddProductAsync(Guid promotionId, CreatePromotionProductRequest request)
        {
            var promotion = await _promotionRepository.GetByIdAsync(promotionId)
                ?? throw new InvalidOperationException("Promotion not found.");

            var product = new PromotionProduct
            {
                Id = Guid.NewGuid(),
                PromotionId = promotionId,
                ProductId = request.ProductId,
                ProductVariantId = request.ProductVariantId,
                IsGetItem = request.IsGetItem,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _promotionRepository.AddProductAsync(product);

            // Re-fetch with includes for the DTO
            var fetched = await _promotionRepository.GetProductByIdAsync(created.Id);
            return MapProductToDto(fetched!);
        }

        public async Task RemoveProductAsync(Guid promotionId, Guid productId)
        {
            var product = await _promotionRepository.GetProductByIdAsync(productId)
                ?? throw new InvalidOperationException("Promotion product not found.");

            if (product.PromotionId != promotionId)
                throw new InvalidOperationException("Product does not belong to this promotion.");

            await _promotionRepository.RemoveProductAsync(product);
        }

        private static PromotionDto MapToDto(Promotion p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code,
            Description = p.Description,
            PromotionType = p.PromotionType,
            Status = p.Status,
            DiscountType = p.DiscountType,
            DiscountValue = p.DiscountValue,
            MaxDiscountAmount = p.MaxDiscountAmount,
            BuyQuantity = p.BuyQuantity,
            GetQuantity = p.GetQuantity,
            MinOrderAmount = p.MinOrderAmount,
            MaxUsageCount = p.MaxUsageCount,
            MaxUsagePerCustomer = p.MaxUsagePerCustomer,
            CurrentUsageCount = p.CurrentUsageCount,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            IsStackable = p.IsStackable,
            Priority = p.Priority,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt
        };

        private static PromotionDetailDto MapToDetailDto(Promotion p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code,
            Description = p.Description,
            PromotionType = p.PromotionType,
            Status = p.Status,
            DiscountType = p.DiscountType,
            DiscountValue = p.DiscountValue,
            MaxDiscountAmount = p.MaxDiscountAmount,
            BuyQuantity = p.BuyQuantity,
            GetQuantity = p.GetQuantity,
            MinOrderAmount = p.MinOrderAmount,
            MaxUsageCount = p.MaxUsageCount,
            MaxUsagePerCustomer = p.MaxUsagePerCustomer,
            CurrentUsageCount = p.CurrentUsageCount,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            IsStackable = p.IsStackable,
            Priority = p.Priority,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            Rules = p.Rules?.Select(MapRuleToDto).ToList() ?? new List<PromotionRuleDto>(),
            Products = p.Products?.Select(MapProductToDto).ToList() ?? new List<PromotionProductDto>()
        };

        private static PromotionRuleDto MapRuleToDto(PromotionRule r) => new()
        {
            Id = r.Id,
            PromotionId = r.PromotionId,
            RuleType = r.RuleType,
            MinQuantity = r.MinQuantity,
            MinOrderAmount = r.MinOrderAmount,
            CustomerGroupId = r.CustomerGroupId,
            CustomerGroupName = r.CustomerGroup?.Name,
            CategoryId = r.CategoryId,
            CategoryName = r.Category?.Name
        };

        private static PromotionProductDto MapProductToDto(PromotionProduct pp) => new()
        {
            Id = pp.Id,
            PromotionId = pp.PromotionId,
            ProductId = pp.ProductId,
            ProductName = pp.Product?.Name,
            ProductVariantId = pp.ProductVariantId,
            VariantName = pp.ProductVariant?.Name,
            IsGetItem = pp.IsGetItem
        };
    }
}
