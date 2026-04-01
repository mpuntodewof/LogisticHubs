using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly AppDbContext _context;

        public LeaveRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<LeaveRequest>> GetPagedAsync(PagedRequest request, Guid? employeeId = null, string? status = null)
        {
            var query = _context.LeaveRequests.Include(l => l.Employee).ThenInclude(e => e.User).AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(l => l.EmployeeId == employeeId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(l => l.Status == status);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(l => l.Employee.User.Name.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "startdate" => request.SortDescending ? query.OrderByDescending(l => l.StartDate) : query.OrderBy(l => l.StartDate),
                "status" => request.SortDescending ? query.OrderByDescending(l => l.Status) : query.OrderBy(l => l.Status),
                _ => query.OrderByDescending(l => l.StartDate)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<LeaveRequest?> GetByIdAsync(Guid id)
            => await _context.LeaveRequests
                .Include(l => l.Employee)
                    .ThenInclude(e => e.User)
                .FirstOrDefaultAsync(l => l.Id == id);

        public async Task<LeaveRequest> CreateAsync(LeaveRequest leaveRequest)
        {
            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();
            return leaveRequest;
        }

        public async Task UpdateAsync(LeaveRequest leaveRequest)
        {
            _context.LeaveRequests.Update(leaveRequest);
            await _context.SaveChangesAsync();
        }
    }
}
