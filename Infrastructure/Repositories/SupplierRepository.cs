using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly AppDbContext _context;

        public SupplierRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Supplier>> GetPagedAsync(PagedRequest request, bool? isActive = null)
        {
            var query = _context.Suppliers.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(s =>
                    s.CompanyName.ToLower().Contains(search) ||
                    s.SupplierCode.ToLower().Contains(search) ||
                    (s.ContactName != null && s.ContactName.ToLower().Contains(search)) ||
                    (s.Email != null && s.Email.ToLower().Contains(search)));
            }

            query = request.SortBy?.ToLower() switch
            {
                "companyname" => request.SortDescending ? query.OrderByDescending(s => s.CompanyName) : query.OrderBy(s => s.CompanyName),
                "code" => request.SortDescending ? query.OrderByDescending(s => s.SupplierCode) : query.OrderBy(s => s.SupplierCode),
                _ => query.OrderBy(s => s.CompanyName)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<Supplier?> GetByIdAsync(Guid id)
            => await _context.Suppliers
                .Include(s => s.PaymentTerm)
                .FirstOrDefaultAsync(s => s.Id == id);

        public async Task<bool> SupplierCodeExistsAsync(string supplierCode)
            => await _context.Suppliers.AnyAsync(s => s.SupplierCode == supplierCode);

        public async Task<Supplier> CreateAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Supplier supplier)
        {
            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
        }
    }
}
