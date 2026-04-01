using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IDashboardWidgetRepository
    {
        Task<PagedResult<DashboardWidget>> GetPagedAsync(PagedRequest request, Guid? userId = null);
        Task<List<DashboardWidget>> GetByUserIdAsync(Guid? userId);
        Task<DashboardWidget?> GetByIdAsync(Guid id);
        Task<DashboardWidget> CreateAsync(DashboardWidget entity);
        Task UpdateAsync(DashboardWidget entity);
        Task DeleteAsync(DashboardWidget entity);
    }
}
