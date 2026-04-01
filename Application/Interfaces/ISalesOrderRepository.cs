using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISalesOrderRepository
    {
        Task<PagedResult<SalesOrder>> GetPagedAsync(PagedRequest request, string? status = null, Guid? customerId = null, Guid? branchId = null);
        Task<SalesOrder?> GetByIdAsync(Guid id);
        Task<SalesOrder?> GetDetailByIdAsync(Guid id);
        Task<bool> OrderNumberExistsAsync(string orderNumber);
        Task<SalesOrder> CreateAsync(SalesOrder order);
        Task UpdateAsync(SalesOrder order);
        Task DeleteAsync(SalesOrder order);
        Task<SalesOrderPayment> AddPaymentAsync(SalesOrderPayment payment);
        Task<IEnumerable<SalesOrderPayment>> GetPaymentsAsync(Guid salesOrderId);
        Task<SalesOrderPayment?> GetPaymentByIdAsync(Guid paymentId);
    }
}
