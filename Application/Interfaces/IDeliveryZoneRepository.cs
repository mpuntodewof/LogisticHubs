using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IDeliveryZoneRepository
    {
        Task<PagedResult<DeliveryZone>> GetPagedAsync(PagedRequest request);
        Task<DeliveryZone?> GetByIdAsync(Guid id);
        Task<bool> CodeExistsAsync(string code);
        Task<DeliveryZone> CreateAsync(DeliveryZone zone);
        Task UpdateAsync(DeliveryZone zone);
        Task DeleteAsync(DeliveryZone zone);
    }
}
