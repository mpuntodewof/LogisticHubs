using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Employee>> GetPagedAsync(PagedRequest request, Guid? departmentId = null, string? status = null)
        {
            var query = _context.Employees.Include(e => e.User).Include(e => e.Department).AsQueryable();

            if (departmentId.HasValue)
                query = query.Where(e => e.DepartmentId == departmentId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(e => e.EmploymentStatus == status);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(e =>
                    e.EmployeeCode.ToLower().Contains(search) ||
                    e.User.Name.ToLower().Contains(search) ||
                    e.User.Email.ToLower().Contains(search) ||
                    e.Position.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "employeecode" => request.SortDescending ? query.OrderByDescending(e => e.EmployeeCode) : query.OrderBy(e => e.EmployeeCode),
                "name" => request.SortDescending ? query.OrderByDescending(e => e.User.Name) : query.OrderBy(e => e.User.Name),
                "position" => request.SortDescending ? query.OrderByDescending(e => e.Position) : query.OrderBy(e => e.Position),
                _ => query.OrderByDescending(e => e.CreatedAt)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<Employee?> GetByIdAsync(Guid id)
            => await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);

        public async Task<Employee?> GetByUserIdAsync(Guid userId)
            => await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);

        public async Task<bool> EmployeeCodeExistsAsync(string employeeCode)
            => await _context.Employees.AnyAsync(e => e.EmployeeCode == employeeCode);

        public async Task<Employee> CreateAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Employee employee)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
    }
}
