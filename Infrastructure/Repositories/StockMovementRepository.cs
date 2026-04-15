using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StockMovementRepository : IStockMovementRepository
    {
        private readonly AppDbContext _context;

        public StockMovementRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<StockMovement>> GetPagedAsync(PagedRequest request, Guid? warehouseId = null, Guid? productVariantId = null, string? movementType = null)
        {
            var query = _context.Set<StockMovement>()
                .Include(m => m.Warehouse)
                .Include(m => m.ProductVariant)
                .AsQueryable();

            if (warehouseId.HasValue)
            {
                query = query.Where(m => m.WarehouseId == warehouseId.Value);
            }

            if (productVariantId.HasValue)
            {
                query = query.Where(m => m.ProductVariantId == productVariantId.Value);
            }

            if (!string.IsNullOrWhiteSpace(movementType))
            {
                query = query.Where(m => m.MovementType == movementType);
            }

            query = query.OrderByDescending(m => m.CreatedAt);

            return await query.ToPagedResultAsync(request);
        }

        public async Task<StockMovement?> GetByIdAsync(Guid id)
            => await _context.Set<StockMovement>()
                .Include(m => m.Warehouse)
                .Include(m => m.ProductVariant)
                .Include(m => m.SourceWarehouse)
                .Include(m => m.DestinationWarehouse)
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<StockMovement> CreateAsync(StockMovement movement)
        {
            _context.Set<StockMovement>().Add(movement);
            return movement;
        }
    }
}
