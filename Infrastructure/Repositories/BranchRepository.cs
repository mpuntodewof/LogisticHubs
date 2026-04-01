using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        private readonly AppDbContext _context;

        public BranchRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Branch>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.Branches.Include(b => b.Warehouse).AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(b =>
                    b.Name.ToLower().Contains(search) ||
                    b.Code.ToLower().Contains(search) ||
                    b.City.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(b => b.Name) : query.OrderBy(b => b.Name),
                "code" => request.SortDescending ? query.OrderByDescending(b => b.Code) : query.OrderBy(b => b.Code),
                _ => query.OrderBy(b => b.Name)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<Branch?> GetByIdAsync(Guid id)
            => await _context.Branches.Include(b => b.Warehouse).FirstOrDefaultAsync(b => b.Id == id);

        public async Task<bool> CodeExistsAsync(string code)
            => await _context.Branches.AnyAsync(b => b.Code == code);

        public async Task<Branch> CreateAsync(Branch branch)
        {
            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();
            return branch;
        }

        public async Task UpdateAsync(Branch branch)
        {
            _context.Branches.Update(branch);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Branch branch)
        {
            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BranchUser>> GetBranchUsersAsync(Guid branchId)
            => await _context.BranchUsers
                .Include(bu => bu.User)
                .Include(bu => bu.Branch)
                .Where(bu => bu.BranchId == branchId)
                .OrderBy(bu => bu.AssignedAt)
                .ToListAsync();

        public async Task AssignUserAsync(BranchUser branchUser)
        {
            _context.BranchUsers.Add(branchUser);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserAsync(Guid branchId, Guid userId)
        {
            var branchUser = await _context.BranchUsers
                .FirstOrDefaultAsync(bu => bu.BranchId == branchId && bu.UserId == userId);

            if (branchUser != null)
            {
                _context.BranchUsers.Remove(branchUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<BranchUser>> GetBranchesByUserIdAsync(Guid userId)
            => await _context.BranchUsers
                .Include(bu => bu.Branch)
                .Include(bu => bu.User)
                .Where(bu => bu.UserId == userId)
                .OrderBy(bu => bu.Branch.Name)
                .ToListAsync();
    }
}
