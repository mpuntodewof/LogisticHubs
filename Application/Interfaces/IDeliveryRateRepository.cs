using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IDeliveryRateRepository
    {
        Task<PagedResult<DeliveryRate>> GetPagedAsync(PagedRequest request, Guid? zoneId = null);
        Task<DeliveryRate?> GetByIdAsync(Guid id);
        Task<IEnumerable<DeliveryRate>> GetByZoneIdAsync(Guid zoneId);
        Task<DeliveryRate> CreateAsync(DeliveryRate rate);
        Task UpdateAsync(DeliveryRate rate);
        Task DeleteAsync(DeliveryRate rate);
    }
}
