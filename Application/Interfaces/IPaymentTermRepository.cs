using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPaymentTermRepository
    {
        Task<PagedResult<PaymentTerm>> GetPagedAsync(PagedRequest request);
        Task<PaymentTerm?> GetByIdAsync(Guid id);
        Task<bool> CodeExistsAsync(string code);
        Task<PaymentTerm> CreateAsync(PaymentTerm paymentTerm);
        Task UpdateAsync(PaymentTerm paymentTerm);
        Task DeleteAsync(PaymentTerm paymentTerm);
    }
}
