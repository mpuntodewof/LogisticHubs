using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class LoyaltyPointTransactionRepository : ILoyaltyPointTransactionRepository
    {
        private readonly AppDbContext _context;

        public LoyaltyPointTransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<LoyaltyPointTransaction>> GetByMembershipIdAsync(Guid membershipId, PagedRequest request)
        {
            var query = _context.LoyaltyPointTransactions
                .Where(t => t.LoyaltyMembershipId == membershipId)
                .OrderByDescending(t => t.TransactionDate)
                .AsQueryable();

            return await query.ToPagedResultAsync(request);
        }

        public async Task<LoyaltyPointTransaction> CreateAsync(LoyaltyPointTransaction transaction)
        {
            _context.LoyaltyPointTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }
    }
}
