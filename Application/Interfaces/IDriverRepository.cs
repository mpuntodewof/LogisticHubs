using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IDriverRepository
    {
        Task<IEnumerable<Driver>> GetAllAsync();
        Task<PagedResult<Driver>> GetPagedAsync(PagedRequest request);
        Task<Driver?> GetByIdAsync(Guid id);
        Task<Driver?> GetByUserIdAsync(Guid userId);
        Task<bool> LicenseNumberExistsAsync(string licenseNumber);
        Task<Driver> CreateAsync(Driver driver);
        Task UpdateAsync(Driver driver);
        Task DeleteAsync(Driver driver);
    }
}
