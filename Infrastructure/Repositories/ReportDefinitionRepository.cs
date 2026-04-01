using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReportDefinitionRepository : IReportDefinitionRepository
    {
        private readonly AppDbContext _context;

        public ReportDefinitionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ReportDefinition>> GetPagedAsync(PagedRequest request, string? reportType = null, bool? isActive = null)
        {
            var query = _context.ReportDefinitions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(e => e.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(reportType))
                query = query.Where(e => e.ReportType == reportType);

            if (isActive.HasValue)
                query = query.Where(e => e.IsActive == isActive.Value);

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(e => e.Name) : query.OrderBy(e => e.Name),
                "reporttype" => request.SortDescending ? query.OrderByDescending(e => e.ReportType) : query.OrderBy(e => e.ReportType),
                _ => query.OrderBy(e => e.Name)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<ReportDefinition?> GetByIdAsync(Guid id)
            => await _context.ReportDefinitions.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<ReportDefinition> CreateAsync(ReportDefinition entity)
        {
            _context.ReportDefinitions.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(ReportDefinition entity)
        {
            _context.ReportDefinitions.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ReportDefinition entity)
        {
            _context.ReportDefinitions.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
