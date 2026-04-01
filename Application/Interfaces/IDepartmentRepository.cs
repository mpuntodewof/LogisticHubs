using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<PagedResult<Department>> GetPagedAsync(PagedRequest request);
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(Guid id);
        Task<bool> CodeExistsAsync(string code);
        Task<Department> CreateAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(Department department);
    }
}
