using Application.DTOs.Common;
using Application.DTOs.HRM;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.HRM
{
    public class DepartmentUseCase
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentUseCase(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<DepartmentDto>> GetPagedAsync(PagedRequest request)
        {
            var paged = await _departmentRepository.GetPagedAsync(request);
            return new PagedResult<DepartmentDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var all = await _departmentRepository.GetAllAsync();
            return all.Select(MapToDto);
        }

        public async Task<DepartmentDto?> GetByIdAsync(Guid id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            return department == null ? null : MapToDto(department);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request)
        {
            if (await _departmentRepository.CodeExistsAsync(request.Code))
                throw new InvalidOperationException($"A department with code '{request.Code}' already exists.");

            if (request.ParentDepartmentId.HasValue)
            {
                _ = await _departmentRepository.GetByIdAsync(request.ParentDepartmentId.Value)
                    ?? throw new KeyNotFoundException($"Parent department {request.ParentDepartmentId.Value} not found.");
            }

            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                ParentDepartmentId = request.ParentDepartmentId,
                ManagerId = request.ManagerId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _departmentRepository.CreateAsync(department);
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateDepartmentRequest request)
        {
            var department = await _departmentRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Department {id} not found.");

            if (request.Name != null) department.Name = request.Name;
            if (request.Description != null) department.Description = request.Description;
            if (request.ParentDepartmentId.HasValue) department.ParentDepartmentId = request.ParentDepartmentId;
            if (request.ManagerId.HasValue) department.ManagerId = request.ManagerId;
            if (request.IsActive.HasValue) department.IsActive = request.IsActive.Value;

            await _departmentRepository.UpdateAsync(department);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var department = await _departmentRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Department {id} not found.");

            await _departmentRepository.DeleteAsync(department);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static DepartmentDto MapToDto(Department d) => new()
        {
            Id = d.Id,
            Name = d.Name,
            Code = d.Code,
            Description = d.Description,
            ParentDepartmentId = d.ParentDepartmentId,
            ParentDepartmentName = d.ParentDepartment?.Name,
            ManagerId = d.ManagerId,
            ManagerName = d.Manager?.User?.Name,
            IsActive = d.IsActive,
            CreatedAt = d.CreatedAt
        };
    }
}
