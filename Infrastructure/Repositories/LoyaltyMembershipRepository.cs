using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class LoyaltyMembershipRepository : ILoyaltyMembershipRepository
    {
        private readonly AppDbContext _context;

        public LoyaltyMembershipRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<LoyaltyMembership>> GetPagedAsync(PagedRequest request, Guid? programId = null, Guid? customerId = null)
        {
            var query = _context.LoyaltyMemberships
                .Include(m => m.LoyaltyProgram)
                .Include(m => m.Customer)
                .Include(m => m.CurrentTier)
                .AsQueryable();

            if (programId.HasValue)
            {
                query = query.Where(m => m.LoyaltyProgramId == programId.Value);
            }

            if (customerId.HasValue)
            {
                query = query.Where(m => m.CustomerId == customerId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(m => m.Customer != null && m.Customer.Name.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "customer" => request.SortDescending ? query.OrderByDescending(m => m.Customer!.Name) : query.OrderBy(m => m.Customer!.Name),
                "points" => request.SortDescending ? query.OrderByDescending(m => m.AvailablePoints) : query.OrderBy(m => m.AvailablePoints),
                "joinedat" => request.SortDescending ? query.OrderByDescending(m => m.JoinedAt) : query.OrderBy(m => m.JoinedAt),
                _ => query.OrderByDescending(m => m.JoinedAt)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<LoyaltyMembership?> GetByIdAsync(Guid id)
            => await _context.LoyaltyMemberships
                .Include(m => m.LoyaltyProgram)
                .Include(m => m.Customer)
                .Include(m => m.CurrentTier)
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<LoyaltyMembership?> GetByCustomerAndProgramAsync(Guid customerId, Guid programId)
            => await _context.LoyaltyMemberships
                .Include(m => m.LoyaltyProgram)
                .Include(m => m.Customer)
                .Include(m => m.CurrentTier)
                .FirstOrDefaultAsync(m => m.CustomerId == customerId && m.LoyaltyProgramId == programId);

        public async Task<LoyaltyMembership> CreateAsync(LoyaltyMembership membership)
        {
            _context.LoyaltyMemberships.Add(membership);
            await _context.SaveChangesAsync();
            return membership;
        }

        public async Task UpdateAsync(LoyaltyMembership membership)
        {
            _context.LoyaltyMemberships.Update(membership);
            await _context.SaveChangesAsync();
        }
    }
}
