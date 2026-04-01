using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGoodsReceiptRepository
    {
        Task<PagedResult<GoodsReceipt>> GetPagedAsync(PagedRequest request, Guid? purchaseOrderId = null);
        Task<GoodsReceipt?> GetByIdAsync(Guid id);
        Task<GoodsReceipt?> GetDetailByIdAsync(Guid id);
        Task<bool> ReceiptNumberExistsAsync(string receiptNumber);
        Task<GoodsReceipt> CreateAsync(GoodsReceipt receipt);
        Task UpdateAsync(GoodsReceipt receipt);
        Task DeleteAsync(GoodsReceipt receipt);
    }
}
