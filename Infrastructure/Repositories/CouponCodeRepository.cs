using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CouponCodeRepository : ICouponCodeRepository
    {
        private readonly AppDbContext _context;

        public CouponCodeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<CouponCode>> GetPagedAsync(PagedRequest request, bool? isActive = null)
        {
            var query = _context.CouponCodes.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(c => c.Code.ToLower().Contains(search)
                    || (c.Description != null && c.Description.ToLower().Contains(search)));
            }

            query = request.SortBy?.ToLower() switch
            {
                "code" => request.SortDescending ? query.OrderByDescending(c => c.Code) : query.OrderBy(c => c.Code),
                "startdate" => request.SortDescending ? query.OrderByDescending(c => c.StartDate) : query.OrderBy(c => c.StartDate),
                "enddate" => request.SortDescending ? query.OrderByDescending(c => c.EndDate) : query.OrderBy(c => c.EndDate),
                _ => query.OrderByDescending(c => c.CreatedAt)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<CouponCode?> GetByIdAsync(Guid id)
            => await _context.CouponCodes.FirstOrDefaultAsync(c => c.Id == id);

        public async Task<CouponCode?> GetByCodeAsync(string code)
            => await _context.CouponCodes.FirstOrDefaultAsync(c => c.Code == code);

        public async Task<bool> CodeExistsAsync(string code)
            => await _context.CouponCodes.AnyAsync(c => c.Code == code);

        public async Task<CouponCode> CreateAsync(CouponCode coupon)
        {
            _context.CouponCodes.Add(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task UpdateAsync(CouponCode coupon)
        {
            _context.CouponCodes.Update(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CouponCode coupon)
        {
            _context.CouponCodes.Remove(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUsageCountByCustomerAsync(Guid couponId, Guid customerId)
            => await _context.CouponUsages
                .CountAsync(u => u.CouponCodeId == couponId && u.CustomerId == customerId);

        public async Task<CouponUsage> AddUsageAsync(CouponUsage usage)
        {
            _context.CouponUsages.Add(usage);
            await _context.SaveChangesAsync();
            return usage;
        }

        public async Task<PagedResult<CouponUsage>> GetUsagesAsync(Guid couponId, PagedRequest request)
        {
            var query = _context.CouponUsages
                .Include(u => u.Customer)
                .Include(u => u.CouponCode)
                .Include(u => u.SalesOrder)
                .Where(u => u.CouponCodeId == couponId)
                .OrderByDescending(u => u.UsedAt)
                .AsQueryable();

            return await query.ToPagedResultAsync(request);
        }
    }
}
