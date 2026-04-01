using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<PagedResult<Attendance>> GetPagedAsync(PagedRequest request, Guid? employeeId = null, DateOnly? date = null);
        Task<Attendance?> GetByIdAsync(Guid id);
        Task<Attendance?> GetOpenAttendanceAsync(Guid employeeId, DateOnly date);
        Task<Attendance> CreateAsync(Attendance attendance);
        Task UpdateAsync(Attendance attendance);
    }
}
