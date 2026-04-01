using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReportExecutionRepository : IReportExecutionRepository
    {
        private readonly AppDbContext _context;

        public ReportExecutionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ReportExecution>> GetPagedAsync(PagedRequest request, Guid? reportDefinitionId = null, string? status = null)
        {
            var query = _context.ReportExecutions
                .Include(e => e.ReportDefinition)
                .AsQueryable();

            if (reportDefinitionId.HasValue)
                query = query.Where(e => e.ReportDefinitionId == reportDefinitionId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(e => e.Status == status);

            query = request.SortBy?.ToLower() switch
            {
                "status" => request.SortDescending ? query.OrderByDescending(e => e.Status) : query.OrderBy(e => e.Status),
                _ => query.OrderByDescending(e => e.CreatedAt)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<ReportExecution?> GetByIdAsync(Guid id)
            => await _context.ReportExecutions
                .Include(e => e.ReportDefinition)
                .FirstOrDefaultAsync(e => e.Id == id);

        public async Task<ReportExecution> CreateAsync(ReportExecution entity)
        {
            _context.ReportExecutions.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(ReportExecution entity)
        {
            _context.ReportExecutions.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
