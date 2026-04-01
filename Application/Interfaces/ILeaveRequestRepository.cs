using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILeaveRequestRepository
    {
        Task<PagedResult<LeaveRequest>> GetPagedAsync(PagedRequest request, Guid? employeeId = null, string? status = null);
        Task<LeaveRequest?> GetByIdAsync(Guid id);
        Task<LeaveRequest> CreateAsync(LeaveRequest leaveRequest);
        Task UpdateAsync(LeaveRequest leaveRequest);
    }
}
