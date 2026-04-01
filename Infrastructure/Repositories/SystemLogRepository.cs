using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SystemLogRepository : ISystemLogRepository
    {
        private readonly AppDbContext _context;

        public SystemLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<SystemLog>> GetPagedAsync(PagedRequest request, string? level = null, string? source = null, Guid? tenantId = null, DateTime? from = null, DateTime? to = null)
        {
            var query = _context.SystemLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(e => e.Message.ToLower().Contains(search) || (e.Source != null && e.Source.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(level))
                query = query.Where(e => e.Level == level);

            if (!string.IsNullOrWhiteSpace(source))
                query = query.Where(e => e.Source == source);

            if (tenantId.HasValue)
                query = query.Where(e => e.TenantId == tenantId.Value);

            if (from.HasValue)
                query = query.Where(e => e.Timestamp >= from.Value);

            if (to.HasValue)
                query = query.Where(e => e.Timestamp <= to.Value);

            query = request.SortBy?.ToLower() switch
            {
                "level" => request.SortDescending ? query.OrderByDescending(e => e.Level) : query.OrderBy(e => e.Level),
                "source" => request.SortDescending ? query.OrderByDescending(e => e.Source) : query.OrderBy(e => e.Source),
                _ => query.OrderByDescending(e => e.Timestamp)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<SystemLog?> GetByIdAsync(Guid id)
            => await _context.SystemLogs.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<SystemLog> CreateAsync(SystemLog entity)
        {
            _context.SystemLogs.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
