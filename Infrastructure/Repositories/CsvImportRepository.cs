using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CsvImportRepository : ICsvImportRepository
    {
        private readonly AppDbContext _context;

        public CsvImportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<CsvImportBatch>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.Set<CsvImportBatch>()
                .Include(b => b.SalesChannel)
                .Include(b => b.Warehouse)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(b => b.FileName.ToLower().Contains(search)
                    || b.SalesChannel.Name.ToLower().Contains(search));
            }

            query = query.OrderByDescending(b => b.CreatedAt);
            return await query.ToPagedResultAsync(request);
        }

        public async Task<CsvImportBatch?> GetByIdAsync(Guid id)
            => await _context.Set<CsvImportBatch>()
                .Include(b => b.SalesChannel)
                .Include(b => b.Warehouse)
                .FirstOrDefaultAsync(b => b.Id == id);

        public async Task<CsvImportBatch?> GetDetailByIdAsync(Guid id)
            => await _context.Set<CsvImportBatch>()
                .Include(b => b.SalesChannel)
                .Include(b => b.Warehouse)
                .Include(b => b.Rows.OrderBy(r => r.RowNumber))
                    .ThenInclude(r => r.MatchedProductVariant)
                .FirstOrDefaultAsync(b => b.Id == id);

        public async Task<CsvImportBatch> CreateAsync(CsvImportBatch batch)
        {
            _context.Set<CsvImportBatch>().Add(batch);
            return batch;
        }

        public async Task UpdateAsync(CsvImportBatch batch)
        {
            // Tracked by change tracker
        }

        public async Task<bool> OrderNumberExistsForChannel(Guid salesChannelId, string orderNumber)
        {
            return await _context.Set<CsvImportRow>()
                .AnyAsync(r => r.Batch.SalesChannelId == salesChannelId
                    && r.OrderNumber == orderNumber
                    && r.Status == "Matched");
        }
    }
}
