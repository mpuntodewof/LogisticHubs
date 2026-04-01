using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class LoyaltyProgramRepository : ILoyaltyProgramRepository
    {
        private readonly AppDbContext _context;

        public LoyaltyProgramRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<LoyaltyProgram>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.LoyaltyPrograms.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "status" => request.SortDescending ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status),
                "createdat" => request.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<LoyaltyProgram?> GetByIdAsync(Guid id)
            => await _context.LoyaltyPrograms.FirstOrDefaultAsync(p => p.Id == id);

        public async Task<LoyaltyProgram?> GetDetailByIdAsync(Guid id)
            => await _context.LoyaltyPrograms
                .Include(p => p.Tiers.OrderBy(t => t.SortOrder))
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<LoyaltyProgram>> GetActiveAsync()
            => await _context.LoyaltyPrograms
                .Where(p => p.IsActive && p.Status == "Active")
                .OrderBy(p => p.Name)
                .ToListAsync();

        public async Task<LoyaltyProgram> CreateAsync(LoyaltyProgram program)
        {
            _context.LoyaltyPrograms.Add(program);
            await _context.SaveChangesAsync();
            return program;
        }

        public async Task UpdateAsync(LoyaltyProgram program)
        {
            _context.LoyaltyPrograms.Update(program);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(LoyaltyProgram program)
        {
            _context.LoyaltyPrograms.Remove(program);
            await _context.SaveChangesAsync();
        }
    }
}
