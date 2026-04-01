using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUnitConversionRepository
    {
        Task<IEnumerable<UnitConversion>> GetByFromUnitAsync(Guid fromUnitId);
        Task<UnitConversion?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid fromUnitId, Guid toUnitId);
        Task<UnitConversion> CreateAsync(UnitConversion conversion);
        Task UpdateAsync(UnitConversion conversion);
        Task DeleteAsync(UnitConversion conversion);
    }
}
