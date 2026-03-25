using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByIdWithRolesAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.UserRoleAssignments)
                    .ThenInclude(ura => ura.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task AssignRoleAsync(Guid userId, Guid roleId, Guid? assignedBy)
        {
            var assignment = new UserRoleAssignment
            {
                UserId = userId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = assignedBy
            };
            _context.UserRoleAssignments.Add(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeRoleAsync(Guid userId, Guid roleId)
        {
            var assignment = await _context.UserRoleAssignments
                .FirstOrDefaultAsync(ura => ura.UserId == userId && ura.RoleId == roleId);

            if (assignment != null)
            {
                _context.UserRoleAssignments.Remove(assignment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Role?> GetRoleByIdAsync(Guid roleId)
        {
            return await _context.Roles.FindAsync(roleId);
        }

        public async Task<bool> UserHasRoleAsync(Guid userId, Guid roleId)
        {
            return await _context.UserRoleAssignments
                .AnyAsync(ura => ura.UserId == userId && ura.RoleId == roleId);
        }
    }
}