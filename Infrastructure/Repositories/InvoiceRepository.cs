using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _context;

        public InvoiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Invoice>> GetPagedAsync(PagedRequest request, string? status = null)
        {
            var query = _context.Invoices
                .Include(i => i.SalesOrder)
                .Include(i => i.Customer)
                .Include(i => i.Branch)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(i => i.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(i => i.InvoiceNumber.ToLower().Contains(search)
                    || (i.TaxInvoiceNumber != null && i.TaxInvoiceNumber.ToLower().Contains(search)));
            }

            query = request.SortBy?.ToLower() switch
            {
                "invoicenumber" => request.SortDescending ? query.OrderByDescending(i => i.InvoiceNumber) : query.OrderBy(i => i.InvoiceNumber),
                "date" => request.SortDescending ? query.OrderByDescending(i => i.InvoiceDate) : query.OrderBy(i => i.InvoiceDate),
                "status" => request.SortDescending ? query.OrderByDescending(i => i.Status) : query.OrderBy(i => i.Status),
                _ => query.OrderByDescending(i => i.InvoiceDate)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<Invoice?> GetByIdAsync(Guid id)
            => await _context.Invoices
                .Include(i => i.SalesOrder)
                .Include(i => i.Customer)
                .Include(i => i.Branch)
                .FirstOrDefaultAsync(i => i.Id == id);

        public async Task<Invoice?> GetDetailByIdAsync(Guid id)
            => await _context.Invoices
                .Include(i => i.Items)
                    .ThenInclude(item => item.TaxRateEntity)
                .Include(i => i.SalesOrder)
                .Include(i => i.Customer)
                .Include(i => i.Branch)
                .Include(i => i.PaymentTerm)
                .FirstOrDefaultAsync(i => i.Id == id);

        public async Task<Invoice?> GetBySalesOrderIdAsync(Guid salesOrderId)
            => await _context.Invoices
                .FirstOrDefaultAsync(i => i.SalesOrderId == salesOrderId);

        public async Task<bool> InvoiceNumberExistsAsync(string invoiceNumber)
            => await _context.Invoices.AnyAsync(i => i.InvoiceNumber == invoiceNumber);

        public async Task<Invoice> CreateAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Invoice invoice)
        {
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
        }
    }
}
