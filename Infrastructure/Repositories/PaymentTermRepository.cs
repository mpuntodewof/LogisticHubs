using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PaymentTermRepository : IPaymentTermRepository
    {
        private readonly AppDbContext _context;

        public PaymentTermRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<PaymentTerm>> GetPagedAsync(PagedRequest request)
        {
            var query = _context.PaymentTerms.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(search)
                    || p.Code.ToLower().Contains(search));
            }

            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "code" => request.SortDescending ? query.OrderByDescending(p => p.Code) : query.OrderBy(p => p.Code),
                _ => query.OrderBy(p => p.Name)
            };

            return await query.ToPagedResultAsync(request);
        }

        public async Task<PaymentTerm?> GetByIdAsync(Guid id)
            => await _context.PaymentTerms.FirstOrDefaultAsync(p => p.Id == id);

        public async Task<bool> CodeExistsAsync(string code)
            => await _context.PaymentTerms.AnyAsync(p => p.Code == code);

        public async Task<PaymentTerm> CreateAsync(PaymentTerm paymentTerm)
        {
            _context.PaymentTerms.Add(paymentTerm);
            return paymentTerm;
        }

        public async Task UpdateAsync(PaymentTerm paymentTerm)
        {
            _context.PaymentTerms.Update(paymentTerm);
        }

        public async Task DeleteAsync(PaymentTerm paymentTerm)
        {
            _context.PaymentTerms.Remove(paymentTerm);
        }
    }
}
