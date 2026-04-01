using Application.DTOs.Common;
using Application.DTOs.Ecommerce;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Ecommerce
{
    public class CouponCodeUseCase
    {
        private readonly ICouponCodeRepository _couponRepository;

        public CouponCodeUseCase(ICouponCodeRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        public async Task<PagedResult<CouponCodeDto>> GetPagedAsync(PagedRequest request, bool? isActive = null)
        {
            var result = await _couponRepository.GetPagedAsync(request, isActive);

            return new PagedResult<CouponCodeDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<CouponCodeDto?> GetByIdAsync(Guid id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            return coupon == null ? null : MapToDto(coupon);
        }

        public async Task<CouponCodeDto> CreateAsync(CreateCouponCodeRequest request)
        {
            var exists = await _couponRepository.CodeExistsAsync(request.Code);
            if (exists)
                throw new InvalidOperationException($"Coupon code '{request.Code}' already exists.");

            if (request.EndDate <= request.StartDate)
                throw new InvalidOperationException("End date must be after start date.");

            var coupon = new CouponCode
            {
                Id = Guid.NewGuid(),
                Code = request.Code.ToUpper(),
                Description = request.Description,
                DiscountType = request.DiscountType.ToString(),
                DiscountValue = request.DiscountValue,
                MinimumOrderAmount = request.MinimumOrderAmount,
                MaxDiscountAmount = request.MaxDiscountAmount,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                MaxUsageCount = request.MaxUsageCount,
                MaxUsagePerCustomer = request.MaxUsagePerCustomer,
                CurrentUsageCount = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _couponRepository.CreateAsync(coupon);
            return MapToDto(created);
        }

        public async Task<CouponCodeDto> UpdateAsync(Guid id, UpdateCouponCodeRequest request)
        {
            var coupon = await _couponRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Coupon code not found.");

            if (request.Description != null) coupon.Description = request.Description;
            if (request.DiscountValue.HasValue) coupon.DiscountValue = request.DiscountValue.Value;
            if (request.MinimumOrderAmount.HasValue) coupon.MinimumOrderAmount = request.MinimumOrderAmount;
            if (request.MaxDiscountAmount.HasValue) coupon.MaxDiscountAmount = request.MaxDiscountAmount;
            if (request.EndDate.HasValue) coupon.EndDate = request.EndDate.Value;
            if (request.MaxUsageCount.HasValue) coupon.MaxUsageCount = request.MaxUsageCount;
            if (request.MaxUsagePerCustomer.HasValue) coupon.MaxUsagePerCustomer = request.MaxUsagePerCustomer.Value;
            if (request.IsActive.HasValue) coupon.IsActive = request.IsActive.Value;

            coupon.UpdatedAt = DateTime.UtcNow;

            await _couponRepository.UpdateAsync(coupon);
            return MapToDto(coupon);
        }

        public async Task DeleteAsync(Guid id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Coupon code not found.");

            await _couponRepository.DeleteAsync(coupon);
        }

        public async Task<ValidateCouponResponse> ValidateAsync(ValidateCouponRequest request, Guid? customerId)
        {
            var coupon = await _couponRepository.GetByCodeAsync(request.Code.ToUpper());

            if (coupon == null)
                return new ValidateCouponResponse { IsValid = false, DiscountAmount = 0, Message = "Coupon code not found." };

            if (!coupon.IsActive)
                return new ValidateCouponResponse { IsValid = false, DiscountAmount = 0, Message = "Coupon code is not active." };

            var now = DateTime.UtcNow;
            if (now < coupon.StartDate || now > coupon.EndDate)
                return new ValidateCouponResponse { IsValid = false, DiscountAmount = 0, Message = "Coupon code is expired or not yet valid." };

            if (coupon.MinimumOrderAmount.HasValue && request.OrderAmount < coupon.MinimumOrderAmount.Value)
                return new ValidateCouponResponse { IsValid = false, DiscountAmount = 0, Message = $"Minimum order amount is {coupon.MinimumOrderAmount.Value:N2}." };

            if (coupon.MaxUsageCount.HasValue && coupon.CurrentUsageCount >= coupon.MaxUsageCount.Value)
                return new ValidateCouponResponse { IsValid = false, DiscountAmount = 0, Message = "Coupon code has reached its maximum usage limit." };

            if (customerId.HasValue)
            {
                var customerUsageCount = await _couponRepository.GetUsageCountByCustomerAsync(coupon.Id, customerId.Value);
                if (customerUsageCount >= coupon.MaxUsagePerCustomer)
                    return new ValidateCouponResponse { IsValid = false, DiscountAmount = 0, Message = "You have reached the maximum usage limit for this coupon." };
            }

            // Calculate discount
            decimal discountAmount;
            if (coupon.DiscountType == DiscountType.Percentage.ToString())
            {
                discountAmount = request.OrderAmount * (coupon.DiscountValue / 100m);
                if (coupon.MaxDiscountAmount.HasValue && discountAmount > coupon.MaxDiscountAmount.Value)
                    discountAmount = coupon.MaxDiscountAmount.Value;
            }
            else
            {
                discountAmount = coupon.DiscountValue;
            }

            // Don't exceed order amount
            if (discountAmount > request.OrderAmount)
                discountAmount = request.OrderAmount;

            return new ValidateCouponResponse
            {
                IsValid = true,
                DiscountAmount = Math.Round(discountAmount, 2),
                Message = "Coupon code is valid."
            };
        }

        public async Task RecordUsageAsync(Guid couponCodeId, Guid customerId, Guid salesOrderId, decimal discountApplied)
        {
            var coupon = await _couponRepository.GetByIdAsync(couponCodeId)
                ?? throw new InvalidOperationException("Coupon code not found.");

            var usage = new CouponUsage
            {
                Id = Guid.NewGuid(),
                CouponCodeId = couponCodeId,
                CustomerId = customerId,
                SalesOrderId = salesOrderId,
                DiscountApplied = discountApplied,
                UsedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _couponRepository.AddUsageAsync(usage);

            coupon.CurrentUsageCount++;
            coupon.UpdatedAt = DateTime.UtcNow;
            await _couponRepository.UpdateAsync(coupon);
        }

        public async Task<PagedResult<CouponUsageDto>> GetUsagesAsync(Guid couponId, PagedRequest request)
        {
            var result = await _couponRepository.GetUsagesAsync(couponId, request);

            return new PagedResult<CouponUsageDto>
            {
                Items = result.Items.Select(MapUsageToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        private static CouponCodeDto MapToDto(CouponCode c) => new()
        {
            Id = c.Id,
            Code = c.Code,
            Description = c.Description,
            DiscountType = c.DiscountType,
            DiscountValue = c.DiscountValue,
            MinimumOrderAmount = c.MinimumOrderAmount,
            MaxDiscountAmount = c.MaxDiscountAmount,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            MaxUsageCount = c.MaxUsageCount,
            MaxUsagePerCustomer = c.MaxUsagePerCustomer,
            CurrentUsageCount = c.CurrentUsageCount,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        };

        private static CouponUsageDto MapUsageToDto(CouponUsage u) => new()
        {
            Id = u.Id,
            CouponCodeId = u.CouponCodeId,
            CouponCode = u.CouponCode?.Code ?? string.Empty,
            CustomerId = u.CustomerId,
            CustomerName = u.Customer?.Name ?? string.Empty,
            SalesOrderId = u.SalesOrderId,
            OrderNumber = u.SalesOrder?.OrderNumber ?? string.Empty,
            DiscountApplied = u.DiscountApplied,
            UsedAt = u.UsedAt
        };
    }
}
