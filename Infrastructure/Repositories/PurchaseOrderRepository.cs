using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly AppDbContext _context;

        public PurchaseOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<PurchaseOrder>> GetPagedAsync(PagedRequest request, string? status = null, Guid? supplierId = null, Guid? warehouseId = null)
        {
            var query = _context.PurchaseOrders
                .Include(o => o.Supplier)
                .Include(o => o.Warehouse)
                .Include(o => o.Branch)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o => o.Status == status);
            }

            if (supplierId.HasValue)
            {
                query = query.Where(o => o.SupplierId == supplierId.Value);
            }

            if (warehouseId.HasValue)
            {
                query = query.Where(o => o.WarehouseId == warehouseId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(o =>
                    o.PoNumber.ToLower().Contains(search) ||
                    o.Supplier.CompanyName.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "ponumber" => request.SortDescending ? query.OrderByDescending(o => o.PoNumber) : query.OrderBy(o => o.PoNumber),
                "date" => request.SortDescending ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate),
                "status" => request.SortDescending ? query.OrderByDescending(o => o.Status) : query.OrderBy(o => o.Status),
                _ => query.OrderByDescending(o => o.OrderDate)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<PurchaseOrder?> GetByIdAsync(Guid id)
            => await _context.PurchaseOrders
                .Include(o => o.Supplier)
                .Include(o => o.Warehouse)
                .Include(o => o.Branch)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<PurchaseOrder?> GetDetailByIdAsync(Guid id)
            => await _context.PurchaseOrders
                .Include(o => o.Items)
                    .ThenInclude(i => i.ProductVariant)
                .Include(o => o.Supplier)
                .Include(o => o.Warehouse)
                .Include(o => o.Branch)
                .Include(o => o.GoodsReceipts)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<bool> PoNumberExistsAsync(string poNumber)
            => await _context.PurchaseOrders.AnyAsync(o => o.PoNumber == poNumber);

        public async Task<PurchaseOrder> CreateAsync(PurchaseOrder order)
        {
            _context.PurchaseOrders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task UpdateAsync(PurchaseOrder order)
        {
            _context.PurchaseOrders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PurchaseOrder order)
        {
            _context.PurchaseOrders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
