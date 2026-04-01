using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TaxRateRepository : ITaxRateRepository
    {
        private readonly AppDbContext _context;

        public TaxRateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<TaxRate>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.TaxRates.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(t => t.Name.ToLower().Contains(search)
                    || t.Code.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name),
                "code" => request.SortDescending ? query.OrderByDescending(t => t.Code) : query.OrderBy(t => t.Code),
                _ => query.OrderBy(t => t.Name)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<TaxRate?> GetByIdAsync(Guid id)
            => await _context.TaxRates.FirstOrDefaultAsync(t => t.Id == id);

        public async Task<bool> CodeExistsAsync(string code)
            => await _context.TaxRates.AnyAsync(t => t.Code == code);

        public async Task<IEnumerable<TaxRate>> GetActiveByProductIdAsync(Guid productId)
        {
            var now = DateTime.UtcNow;

            return await _context.ProductTaxRates
                .Where(pt => pt.ProductId == productId)
                .Include(pt => pt.TaxRate)
                .Select(pt => pt.TaxRate)
                .Where(t => t.IsActive
                    && t.EffectiveFrom <= now
                    && (t.EffectiveTo == null || t.EffectiveTo >= now))
                .ToListAsync();
        }

        public async Task<TaxRate> CreateAsync(TaxRate taxRate)
        {
            _context.TaxRates.Add(taxRate);
            await _context.SaveChangesAsync();
            return taxRate;
        }

        public async Task UpdateAsync(TaxRate taxRate)
        {
            _context.TaxRates.Update(taxRate);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaxRate taxRate)
        {
            _context.TaxRates.Remove(taxRate);
            await _context.SaveChangesAsync();
        }

        public async Task AssignToProductAsync(Guid productId, Guid taxRateId, Guid tenantId)
        {
            var productTaxRate = new ProductTaxRate
            {
                ProductId = productId,
                TaxRateId = taxRateId,
                TenantId = tenantId
            };

            _context.ProductTaxRates.Add(productTaxRate);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromProductAsync(Guid productId, Guid taxRateId)
        {
            var productTaxRate = await _context.ProductTaxRates
                .FirstOrDefaultAsync(pt => pt.ProductId == productId && pt.TaxRateId == taxRateId);

            if (productTaxRate != null)
            {
                _context.ProductTaxRates.Remove(productTaxRate);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProductTaxRate>> GetProductTaxRatesAsync(Guid productId)
            => await _context.ProductTaxRates
                .Where(pt => pt.ProductId == productId)
                .Include(pt => pt.TaxRate)
                .ToListAsync();
    }
}
