using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISystemLogRepository
    {
        Task<PagedResult<SystemLog>> GetPagedAsync(PagedRequest request, string? level = null, string? source = null, Guid? tenantId = null, DateTime? from = null, DateTime? to = null);
        Task<SystemLog?> GetByIdAsync(Guid id);
        Task<SystemLog> CreateAsync(SystemLog entity);
    }
}
