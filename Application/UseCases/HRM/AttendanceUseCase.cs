using Application.DTOs.Common;
using Application.DTOs.HRM;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.HRM
{
    public class AttendanceUseCase
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceUseCase(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<AttendanceDto>> GetPagedAsync(PagedRequest request, Guid? employeeId = null, DateOnly? date = null)
        {
            var paged = await _attendanceRepository.GetPagedAsync(request, employeeId, date);
            return new PagedResult<AttendanceDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        // ── Clock In ─────────────────────────────────────────────────────────────

        public async Task<AttendanceDto> ClockInAsync(ClockInRequest request)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var existing = await _attendanceRepository.GetOpenAttendanceAsync(request.EmployeeId, today);
            if (existing != null)
                throw new InvalidOperationException("Employee already has an open attendance record for today. Please clock out first.");

            var attendance = new Attendance
            {
                Id = Guid.NewGuid(),
                EmployeeId = request.EmployeeId,
                Date = today,
                ClockIn = DateTime.UtcNow,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _attendanceRepository.CreateAsync(attendance);
            return MapToDto(created);
        }

        // ── Clock Out ────────────────────────────────────────────────────────────

        public async Task<AttendanceDto> ClockOutAsync(Guid attendanceId)
        {
            var attendance = await _attendanceRepository.GetByIdAsync(attendanceId)
                ?? throw new KeyNotFoundException($"Attendance {attendanceId} not found.");

            if (attendance.ClockOut != null)
                throw new InvalidOperationException("This attendance record has already been clocked out.");

            attendance.ClockOut = DateTime.UtcNow;
            attendance.WorkingHours = Math.Round((decimal)(attendance.ClockOut.Value - attendance.ClockIn).TotalHours, 2);

            await _attendanceRepository.UpdateAsync(attendance);
            return MapToDto(attendance);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static AttendanceDto MapToDto(Attendance a) => new()
        {
            Id = a.Id,
            EmployeeId = a.EmployeeId,
            EmployeeName = a.Employee?.User?.Name,
            Date = a.Date,
            ClockIn = a.ClockIn,
            ClockOut = a.ClockOut,
            WorkingHours = a.WorkingHours,
            Notes = a.Notes
        };
    }
}
