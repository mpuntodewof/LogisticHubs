using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;

        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Department>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.Departments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(d => d.Name.ToLower().Contains(search) || d.Code.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name),
                "code" => request.SortDescending ? query.OrderByDescending(d => d.Code) : query.OrderBy(d => d.Code),
                _ => query.OrderBy(d => d.Name)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
            => await _context.Departments.Where(d => d.IsActive).OrderBy(d => d.Name).ToListAsync();

        public async Task<Department?> GetByIdAsync(Guid id)
            => await _context.Departments
                .Include(d => d.ParentDepartment)
                .Include(d => d.Manager!)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(d => d.Id == id);

        public async Task<bool> CodeExistsAsync(string code)
            => await _context.Departments.AnyAsync(d => d.Code == code);

        public async Task<Department> CreateAsync(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Department department)
        {
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }
    }
}
