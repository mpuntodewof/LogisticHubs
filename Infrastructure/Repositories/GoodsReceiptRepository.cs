using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GoodsReceiptRepository : IGoodsReceiptRepository
    {
        private readonly AppDbContext _context;

        public GoodsReceiptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<GoodsReceipt>> GetPagedAsync(PagedRequest request, Guid? purchaseOrderId = null)
        {
            var query = _context.GoodsReceipts
                .Include(r => r.PurchaseOrder)
                .Include(r => r.Warehouse)
                .AsQueryable();

            if (purchaseOrderId.HasValue)
            {
                query = query.Where(r => r.PurchaseOrderId == purchaseOrderId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(r => r.ReceiptNumber.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "receiptnumber" => request.SortDescending ? query.OrderByDescending(r => r.ReceiptNumber) : query.OrderBy(r => r.ReceiptNumber),
                "date" => request.SortDescending ? query.OrderByDescending(r => r.ReceivedDate) : query.OrderBy(r => r.ReceivedDate),
                _ => query.OrderByDescending(r => r.ReceivedDate)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<GoodsReceipt?> GetByIdAsync(Guid id)
            => await _context.GoodsReceipts
                .Include(r => r.PurchaseOrder)
                .Include(r => r.Warehouse)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<GoodsReceipt?> GetDetailByIdAsync(Guid id)
            => await _context.GoodsReceipts
                .Include(r => r.Items)
                    .ThenInclude(i => i.PurchaseOrderItem)
                        .ThenInclude(poi => poi.ProductVariant)
                .Include(r => r.PurchaseOrder)
                .Include(r => r.Warehouse)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<bool> ReceiptNumberExistsAsync(string receiptNumber)
            => await _context.GoodsReceipts.AnyAsync(r => r.ReceiptNumber == receiptNumber);

        public async Task<GoodsReceipt> CreateAsync(GoodsReceipt receipt)
        {
            _context.GoodsReceipts.Add(receipt);
            await _context.SaveChangesAsync();
            return receipt;
        }

        public async Task UpdateAsync(GoodsReceipt receipt)
        {
            _context.GoodsReceipts.Update(receipt);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(GoodsReceipt receipt)
        {
            _context.GoodsReceipts.Remove(receipt);
            await _context.SaveChangesAsync();
        }
    }
}
