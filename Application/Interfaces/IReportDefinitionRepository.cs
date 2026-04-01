using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IReportDefinitionRepository
    {
        Task<PagedResult<ReportDefinition>> GetPagedAsync(PagedRequest request, string? reportType = null, bool? isActive = null);
        Task<ReportDefinition?> GetByIdAsync(Guid id);
        Task<ReportDefinition> CreateAsync(ReportDefinition entity);
        Task UpdateAsync(ReportDefinition entity);
        Task DeleteAsync(ReportDefinition entity);
    }
}
