using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PaymentGatewayConfigRepository : IPaymentGatewayConfigRepository
    {
        private readonly AppDbContext _context;

        public PaymentGatewayConfigRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<PaymentGatewayConfig>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.PaymentGatewayConfigs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(search)
                    || c.Provider.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                "provider" => request.SortDescending ? query.OrderByDescending(c => c.Provider) : query.OrderBy(c => c.Provider),
                _ => query.OrderBy(c => c.Name)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<PaymentGatewayConfig?> GetByIdAsync(Guid id)
            => await _context.PaymentGatewayConfigs.FirstOrDefaultAsync(c => c.Id == id);

        public async Task<PaymentGatewayConfig?> GetActiveByProviderAsync(string provider)
            => await _context.PaymentGatewayConfigs
                .Where(c => c.Provider == provider && c.IsActive)
                .FirstOrDefaultAsync();

        public async Task<PaymentGatewayConfig> CreateAsync(PaymentGatewayConfig config)
        {
            _context.PaymentGatewayConfigs.Add(config);
            await _context.SaveChangesAsync();
            return config;
        }

        public async Task UpdateAsync(PaymentGatewayConfig config)
        {
            _context.PaymentGatewayConfigs.Update(config);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PaymentGatewayConfig config)
        {
            _context.PaymentGatewayConfigs.Remove(config);
            await _context.SaveChangesAsync();
        }
    }
}
