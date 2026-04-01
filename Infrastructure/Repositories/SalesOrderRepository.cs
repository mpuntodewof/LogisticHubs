using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly AppDbContext _context;

        public SalesOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<SalesOrder>> GetPagedAsync(PagedRequest request, string? status = null, Guid? customerId = null, Guid? branchId = null)
        {
            var query = _context.SalesOrders
                .Include(o => o.Customer)
                .Include(o => o.Branch)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o => o.Status == status);
            }

            if (customerId.HasValue)
            {
                query = query.Where(o => o.CustomerId == customerId.Value);
            }

            if (branchId.HasValue)
            {
                query = query.Where(o => o.BranchId == branchId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(o => o.OrderNumber.ToLower().Contains(search)
                    || (o.Customer != null && o.Customer.Name.ToLower().Contains(search)));
            }

            query = request.SortBy?.ToLower() switch
            {
                "ordernumber" => request.SortDescending ? query.OrderByDescending(o => o.OrderNumber) : query.OrderBy(o => o.OrderNumber),
                "date" => request.SortDescending ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate),
                "status" => request.SortDescending ? query.OrderByDescending(o => o.Status) : query.OrderBy(o => o.Status),
                "total" => request.SortDescending ? query.OrderByDescending(o => o.GrandTotal) : query.OrderBy(o => o.GrandTotal),
                _ => query.OrderByDescending(o => o.OrderDate)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<SalesOrder?> GetByIdAsync(Guid id)
            => await _context.SalesOrders
                .Include(o => o.Customer)
                .Include(o => o.Branch)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<SalesOrder?> GetDetailByIdAsync(Guid id)
            => await _context.SalesOrders
                .Include(o => o.Items)
                    .ThenInclude(i => i.ProductVariant)
                .Include(o => o.Payments)
                .Include(o => o.Customer)
                .Include(o => o.Branch)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<bool> OrderNumberExistsAsync(string orderNumber)
            => await _context.SalesOrders.AnyAsync(o => o.OrderNumber == orderNumber);

        public async Task<SalesOrder> CreateAsync(SalesOrder order)
        {
            _context.SalesOrders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task UpdateAsync(SalesOrder order)
        {
            _context.SalesOrders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SalesOrder order)
        {
            _context.SalesOrders.Remove(order);
            await _context.SaveChangesAsync();
        }

        public async Task<SalesOrderPayment> AddPaymentAsync(SalesOrderPayment payment)
        {
            _context.SalesOrderPayments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<IEnumerable<SalesOrderPayment>> GetPaymentsAsync(Guid salesOrderId)
            => await _context.SalesOrderPayments
                .Where(p => p.SalesOrderId == salesOrderId)
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync();

        public async Task<SalesOrderPayment?> GetPaymentByIdAsync(Guid paymentId)
            => await _context.SalesOrderPayments.FirstOrDefaultAsync(p => p.Id == paymentId);
    }
}
