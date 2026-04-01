using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<AuditLog>> GetPagedAsync(PagedRequest request, Guid? userId = null, string? entityType = null, string? action = null, DateTime? from = null, DateTime? to = null)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(e => e.EntityType.ToLower().Contains(search) || (e.UserEmail != null && e.UserEmail.ToLower().Contains(search)));
            }

            if (userId.HasValue)
                query = query.Where(e => e.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(entityType))
                query = query.Where(e => e.EntityType == entityType);

            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(e => e.Action == action);

            if (from.HasValue)
                query = query.Where(e => e.Timestamp >= from.Value);

            if (to.HasValue)
                query = query.Where(e => e.Timestamp <= to.Value);

            query = request.SortBy?.ToLower() switch
            {
                "action" => request.SortDescending ? query.OrderByDescending(e => e.Action) : query.OrderBy(e => e.Action),
                "entitytype" => request.SortDescending ? query.OrderByDescending(e => e.EntityType) : query.OrderBy(e => e.EntityType),
                _ => query.OrderByDescending(e => e.Timestamp)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<AuditLog?> GetByIdAsync(Guid id)
            => await _context.AuditLogs.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<AuditLog> CreateAsync(AuditLog entity)
        {
            _context.AuditLogs.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
