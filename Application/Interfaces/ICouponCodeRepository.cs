using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICouponCodeRepository
    {
        Task<PagedResult<CouponCode>> GetPagedAsync(PagedRequest request, bool? isActive = null);
        Task<CouponCode?> GetByIdAsync(Guid id);
        Task<CouponCode?> GetByCodeAsync(string code);
        Task<bool> CodeExistsAsync(string code);
        Task<CouponCode> CreateAsync(CouponCode coupon);
        Task UpdateAsync(CouponCode coupon);
        Task DeleteAsync(CouponCode coupon);
        Task<int> GetUsageCountByCustomerAsync(Guid couponId, Guid customerId);
        Task<CouponUsage> AddUsageAsync(CouponUsage usage);
        Task<PagedResult<CouponUsage>> GetUsagesAsync(Guid couponId, PagedRequest request);
    }
}
