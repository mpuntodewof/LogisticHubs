using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class WarehouseStockRepository : IWarehouseStockRepository
    {
        private readonly AppDbContext _context;

        public WarehouseStockRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<WarehouseStock>> GetPagedAsync(PagedRequest request, Guid? warehouseId = null, Guid? productVariantId = null)
        {
            var query = _context.Set<WarehouseStock>()
                .Include(s => s.Warehouse)
                .Include(s => s.ProductVariant)
                    .ThenInclude(v => v.Product)
                .AsQueryable();

            if (warehouseId.HasValue)
            {
                query = query.Where(s => s.WarehouseId == warehouseId.Value);
            }

            if (productVariantId.HasValue)
            {
                query = query.Where(s => s.ProductVariantId == productVariantId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(s => s.ProductVariant.Sku.ToLower().Contains(search) || s.Warehouse.Name.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "warehouse" => request.SortDescending ? query.OrderByDescending(s => s.Warehouse.Name) : query.OrderBy(s => s.Warehouse.Name),
                "sku" => request.SortDescending ? query.OrderByDescending(s => s.ProductVariant.Sku) : query.OrderBy(s => s.ProductVariant.Sku),
                _ => query.OrderBy(s => s.Warehouse.Name).ThenBy(s => s.ProductVariant.Sku)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<WarehouseStock?> GetByIdAsync(Guid id)
            => await _context.Set<WarehouseStock>()
                .Include(s => s.Warehouse)
                .Include(s => s.ProductVariant)
                .FirstOrDefaultAsync(s => s.Id == id);

        public async Task<WarehouseStock?> GetByWarehouseAndVariantAsync(Guid warehouseId, Guid productVariantId)
            => await _context.Set<WarehouseStock>()
                .FirstOrDefaultAsync(s => s.WarehouseId == warehouseId && s.ProductVariantId == productVariantId);

        public async Task<IEnumerable<WarehouseStock>> GetLowStockAsync(Guid? warehouseId = null)
        {
            var query = _context.Set<WarehouseStock>()
                .Include(s => s.Warehouse)
                .Include(s => s.ProductVariant)
                .Where(s => s.ReorderPoint != null && s.QuantityOnHand <= s.ReorderPoint)
                .AsQueryable();

            if (warehouseId.HasValue)
            {
                query = query.Where(s => s.WarehouseId == warehouseId.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<WarehouseStock> CreateAsync(WarehouseStock stock)
        {
            _context.Set<WarehouseStock>().Add(stock);
            await _context.SaveChangesAsync();
            return stock;
        }

        public async Task UpdateAsync(WarehouseStock stock)
        {
            _context.Set<WarehouseStock>().Update(stock);
            await _context.SaveChangesAsync();
        }
    }
}
