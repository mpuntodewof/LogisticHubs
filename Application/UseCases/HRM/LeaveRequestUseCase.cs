using Application.DTOs.Common;
using Application.DTOs.HRM;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.HRM
{
    public class LeaveRequestUseCase
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ICurrentUserService _currentUserService;

        public LeaveRequestUseCase(ILeaveRequestRepository leaveRequestRepository, ICurrentUserService currentUserService)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _currentUserService = currentUserService;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<LeaveRequestDto>> GetPagedAsync(PagedRequest request, Guid? employeeId = null, string? status = null)
        {
            var paged = await _leaveRequestRepository.GetPagedAsync(request, employeeId, status);
            return new PagedResult<LeaveRequestDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<LeaveRequestDto?> GetByIdAsync(Guid id)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id);
            return leaveRequest == null ? null : MapToDto(leaveRequest);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<LeaveRequestDto> CreateAsync(CreateLeaveRequestRequest request)
        {
            if (request.EndDate < request.StartDate)
                throw new InvalidOperationException("End date must be on or after start date.");

            var totalDays = (request.EndDate - request.StartDate).Days + 1;

            var leaveRequest = new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = request.EmployeeId,
                LeaveType = request.LeaveType.ToString(),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalDays = totalDays,
                Status = LeaveRequestStatus.Pending.ToString(),
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _leaveRequestRepository.CreateAsync(leaveRequest);
            return MapToDto(created);
        }

        // ── Approve ──────────────────────────────────────────────────────────────

        public async Task ApproveAsync(Guid id)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Leave request {id} not found.");

            if (leaveRequest.Status != LeaveRequestStatus.Pending.ToString())
                throw new InvalidOperationException("Only pending leave requests can be approved.");

            leaveRequest.Status = LeaveRequestStatus.Approved.ToString();
            leaveRequest.ApprovedBy = _currentUserService.UserId;
            leaveRequest.ApprovedAt = DateTime.UtcNow;

            await _leaveRequestRepository.UpdateAsync(leaveRequest);
        }

        // ── Reject ───────────────────────────────────────────────────────────────

        public async Task RejectAsync(Guid id, string rejectionReason)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Leave request {id} not found.");

            if (leaveRequest.Status != LeaveRequestStatus.Pending.ToString())
                throw new InvalidOperationException("Only pending leave requests can be rejected.");

            leaveRequest.Status = LeaveRequestStatus.Rejected.ToString();
            leaveRequest.ApprovedBy = _currentUserService.UserId;
            leaveRequest.ApprovedAt = DateTime.UtcNow;
            leaveRequest.RejectionReason = rejectionReason;

            await _leaveRequestRepository.UpdateAsync(leaveRequest);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static LeaveRequestDto MapToDto(LeaveRequest l) => new()
        {
            Id = l.Id,
            EmployeeId = l.EmployeeId,
            EmployeeName = l.Employee?.User?.Name,
            LeaveType = l.LeaveType,
            StartDate = l.StartDate,
            EndDate = l.EndDate,
            TotalDays = l.TotalDays,
            Status = l.Status,
            Reason = l.Reason,
            ApprovedBy = l.ApprovedBy,
            ApprovedAt = l.ApprovedAt,
            RejectionReason = l.RejectionReason,
            CreatedAt = l.CreatedAt
        };
    }
}
