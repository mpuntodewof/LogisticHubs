using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<PagedResult<Invoice>> GetPagedAsync(PagedRequest request, string? status = null);
        Task<Invoice?> GetByIdAsync(Guid id);
        Task<Invoice?> GetDetailByIdAsync(Guid id);
        Task<Invoice?> GetBySalesOrderIdAsync(Guid salesOrderId);
        Task<bool> InvoiceNumberExistsAsync(string invoiceNumber);
        Task<Invoice> CreateAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task DeleteAsync(Invoice invoice);
    }
}
