using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<PagedResult<Vehicle>> GetPagedAsync(PagedRequest request);
        Task<Vehicle?> GetByIdAsync(Guid id);
        Task<bool> PlateNumberExistsAsync(string plateNumber);
        Task<Vehicle> CreateAsync(Vehicle vehicle);
        Task UpdateAsync(Vehicle vehicle);
        Task DeleteAsync(Vehicle vehicle);
    }
}
