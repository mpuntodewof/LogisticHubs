using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _context;

        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Attendance>> GetPagedAsync(PagedRequest request, Guid? employeeId = null, DateOnly? date = null)
        {
            var query = _context.Attendances.Include(a => a.Employee).ThenInclude(e => e.User).AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(a => a.EmployeeId == employeeId.Value);

            if (date.HasValue)
                query = query.Where(a => a.Date == date.Value);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(a => a.Employee.User.Name.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "date" => request.SortDescending ? query.OrderByDescending(a => a.Date) : query.OrderBy(a => a.Date),
                "clockin" => request.SortDescending ? query.OrderByDescending(a => a.ClockIn) : query.OrderBy(a => a.ClockIn),
                _ => query.OrderByDescending(a => a.Date).ThenByDescending(a => a.ClockIn)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<Attendance?> GetByIdAsync(Guid id)
            => await _context.Attendances
                .Include(a => a.Employee)
                    .ThenInclude(e => e.User)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<Attendance?> GetOpenAttendanceAsync(Guid employeeId, DateOnly date)
            => await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == date && a.ClockOut == null);

        public async Task<Attendance> CreateAsync(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        public async Task UpdateAsync(Attendance attendance)
        {
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
        }
    }
}
