using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<PagedResult<AuditLog>> GetPagedAsync(PagedRequest request, Guid? userId = null, string? entityType = null, string? action = null, DateTime? from = null, DateTime? to = null);
        Task<AuditLog?> GetByIdAsync(Guid id);
        Task<AuditLog> CreateAsync(AuditLog entity);
    }
}
