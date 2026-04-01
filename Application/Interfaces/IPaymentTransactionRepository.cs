using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPaymentTransactionRepository
    {
        Task<PagedResult<PaymentTransaction>> GetPagedAsync(PagedRequest request, Guid? salesOrderPaymentId = null, string? status = null);
        Task<PaymentTransaction?> GetByIdAsync(Guid id);
        Task<PaymentTransaction?> GetDetailByIdAsync(Guid id);
        Task<IEnumerable<PaymentTransaction>> GetBySalesOrderPaymentIdAsync(Guid salesOrderPaymentId);
        Task<PaymentTransaction?> GetByExternalTransactionIdAsync(string externalTransactionId);
        Task<PaymentTransaction> CreateAsync(PaymentTransaction transaction);
        Task UpdateAsync(PaymentTransaction transaction);
    }
}
