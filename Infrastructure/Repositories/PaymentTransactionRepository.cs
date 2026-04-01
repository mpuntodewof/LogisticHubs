using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly AppDbContext _context;

        public PaymentTransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<PaymentTransaction>> GetPagedAsync(PagedRequest request, Guid? salesOrderPaymentId = null, string? status = null)
        {
            var query = _context.PaymentTransactions.AsQueryable();

            if (salesOrderPaymentId.HasValue)
            {
                query = query.Where(t => t.SalesOrderPaymentId == salesOrderPaymentId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(t => t.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(t => t.TransactionNumber.ToLower().Contains(search)
                    || (t.ExternalTransactionId != null && t.ExternalTransactionId.ToLower().Contains(search)));
            }

            query = request.SortBy?.ToLower() switch
            {
                "transactionnumber" => request.SortDescending ? query.OrderByDescending(t => t.TransactionNumber) : query.OrderBy(t => t.TransactionNumber),
                "date" => request.SortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
                "status" => request.SortDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
                _ => query.OrderByDescending(t => t.CreatedAt)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<PaymentTransaction?> GetByIdAsync(Guid id)
            => await _context.PaymentTransactions.FirstOrDefaultAsync(t => t.Id == id);

        public async Task<PaymentTransaction?> GetDetailByIdAsync(Guid id)
            => await _context.PaymentTransactions
                .Include(t => t.SalesOrderPayment)
                .Include(t => t.PaymentGatewayConfig)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<IEnumerable<PaymentTransaction>> GetBySalesOrderPaymentIdAsync(Guid salesOrderPaymentId)
            => await _context.PaymentTransactions
                .Where(t => t.SalesOrderPaymentId == salesOrderPaymentId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

        public async Task<PaymentTransaction?> GetByExternalTransactionIdAsync(string externalTransactionId)
            => await _context.PaymentTransactions
                .FirstOrDefaultAsync(t => t.ExternalTransactionId == externalTransactionId);

        public async Task<PaymentTransaction> CreateAsync(PaymentTransaction transaction)
        {
            _context.PaymentTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task UpdateAsync(PaymentTransaction transaction)
        {
            _context.PaymentTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
