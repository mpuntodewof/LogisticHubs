using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IReportExecutionRepository
    {
        Task<PagedResult<ReportExecution>> GetPagedAsync(PagedRequest request, Guid? reportDefinitionId = null, string? status = null);
        Task<ReportExecution?> GetByIdAsync(Guid id);
        Task<ReportExecution> CreateAsync(ReportExecution entity);
        Task UpdateAsync(ReportExecution entity);
    }
}
