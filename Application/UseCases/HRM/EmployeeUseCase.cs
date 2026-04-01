using Application.DTOs.Common;
using Application.DTOs.HRM;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.HRM
{
    public class EmployeeUseCase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeUseCase(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<EmployeeDto>> GetPagedAsync(PagedRequest request, Guid? departmentId = null, string? status = null)
        {
            var paged = await _employeeRepository.GetPagedAsync(request, departmentId, status);
            return new PagedResult<EmployeeDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<EmployeeDetailDto?> GetByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            return employee == null ? null : MapToDetailDto(employee);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request)
        {
            var existing = await _employeeRepository.GetByUserIdAsync(request.UserId);
            if (existing != null)
                throw new InvalidOperationException($"User {request.UserId} is already linked to employee {existing.EmployeeCode}.");

            var employeeCode = await GenerateEmployeeCodeAsync();

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                EmployeeCode = employeeCode,
                UserId = request.UserId,
                DepartmentId = request.DepartmentId,
                Position = request.Position,
                EmploymentStatus = "Active",
                HireDate = request.HireDate,
                BaseSalary = request.BaseSalary,
                BankName = request.BankName,
                BankAccountNumber = request.BankAccountNumber,
                BankAccountName = request.BankAccountName,
                Phone = request.Phone,
                Address = request.Address,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _employeeRepository.CreateAsync(employee);
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateEmployeeRequest request)
        {
            var employee = await _employeeRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Employee {id} not found.");

            if (request.DepartmentId.HasValue) employee.DepartmentId = request.DepartmentId;
            if (request.Position != null) employee.Position = request.Position;
            if (request.EmploymentStatus.HasValue) employee.EmploymentStatus = request.EmploymentStatus.Value.ToString();
            if (request.BaseSalary.HasValue) employee.BaseSalary = request.BaseSalary.Value;
            if (request.BankName != null) employee.BankName = request.BankName;
            if (request.BankAccountNumber != null) employee.BankAccountNumber = request.BankAccountNumber;
            if (request.BankAccountName != null) employee.BankAccountName = request.BankAccountName;
            if (request.Phone != null) employee.Phone = request.Phone;
            if (request.Address != null) employee.Address = request.Address;
            if (request.Notes != null) employee.Notes = request.Notes;

            await _employeeRepository.UpdateAsync(employee);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Employee {id} not found.");

            await _employeeRepository.DeleteAsync(employee);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private async Task<string> GenerateEmployeeCodeAsync()
        {
            var random = new Random();
            string code;

            do
            {
                var suffix = random.Next(0, 10000).ToString("D4");
                code = $"EMP-{suffix}";
            }
            while (await _employeeRepository.EmployeeCodeExistsAsync(code));

            return code;
        }

        private static EmployeeDto MapToDto(Employee e) => new()
        {
            Id = e.Id,
            EmployeeCode = e.EmployeeCode,
            UserId = e.UserId,
            UserName = e.User?.Name,
            UserEmail = e.User?.Email,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department?.Name,
            Position = e.Position,
            EmploymentStatus = e.EmploymentStatus,
            HireDate = e.HireDate,
            BaseSalary = e.BaseSalary,
            CreatedAt = e.CreatedAt
        };

        private static EmployeeDetailDto MapToDetailDto(Employee e) => new()
        {
            Id = e.Id,
            EmployeeCode = e.EmployeeCode,
            UserId = e.UserId,
            UserName = e.User?.Name,
            UserEmail = e.User?.Email,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department?.Name,
            Position = e.Position,
            EmploymentStatus = e.EmploymentStatus,
            HireDate = e.HireDate,
            BaseSalary = e.BaseSalary,
            CreatedAt = e.CreatedAt,
            TerminationDate = e.TerminationDate,
            BankName = e.BankName,
            BankAccountNumber = e.BankAccountNumber,
            BankAccountName = e.BankAccountName,
            Phone = e.Phone,
            Address = e.Address,
            Notes = e.Notes
        };
    }
}
