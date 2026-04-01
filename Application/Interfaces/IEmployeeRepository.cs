using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<PagedResult<Employee>> GetPagedAsync(PagedRequest request, Guid? departmentId = null, string? status = null);
        Task<Employee?> GetByIdAsync(Guid id);
        Task<Employee?> GetByUserIdAsync(Guid userId);
        Task<bool> EmployeeCodeExistsAsync(string employeeCode);
        Task<Employee> CreateAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(Employee employee);
    }
}
