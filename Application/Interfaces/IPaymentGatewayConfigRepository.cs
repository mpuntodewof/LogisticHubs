using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPaymentGatewayConfigRepository
    {
        Task<PagedResult<PaymentGatewayConfig>> GetPagedAsync(PagedRequest request);
        Task<PaymentGatewayConfig?> GetByIdAsync(Guid id);
        Task<PaymentGatewayConfig?> GetActiveByProviderAsync(string provider);
        Task<PaymentGatewayConfig> CreateAsync(PaymentGatewayConfig config);
        Task UpdateAsync(PaymentGatewayConfig config);
        Task DeleteAsync(PaymentGatewayConfig config);
    }
}
