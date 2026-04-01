using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUnitOfMeasureRepository
    {
        Task<IEnumerable<UnitOfMeasure>> GetAllAsync();
        Task<PagedResult<UnitOfMeasure>> GetPagedAsync(PagedRequest request);
        Task<UnitOfMeasure?> GetByIdAsync(Guid id);
        Task<bool> AbbreviationExistsAsync(string abbreviation);
        Task<bool> IsInUseByProductsAsync(Guid id);
        Task<UnitOfMeasure> CreateAsync(UnitOfMeasure unit);
        Task UpdateAsync(UnitOfMeasure unit);
        Task DeleteAsync(UnitOfMeasure unit);
    }
}
