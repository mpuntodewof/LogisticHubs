using Application.DTOs.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISalesChannelRepository
    {
        Task<PagedResult<SalesChannel>> GetPagedAsync(PagedRequest request);
        Task<SalesChannel?> GetByIdAsync(Guid id);
        Task<SalesChannel?> GetBySlugAsync(string slug);
        Task<SalesChannel> CreateAsync(SalesChannel channel);
        Task UpdateAsync(SalesChannel channel);
        Task DeleteAsync(SalesChannel channel);
    }

    public interface ICsvImportRepository
    {
        Task<PagedResult<CsvImportBatch>> GetPagedAsync(PagedRequest request);
        Task<CsvImportBatch?> GetByIdAsync(Guid id);
        Task<CsvImportBatch?> GetDetailByIdAsync(Guid id);
        Task<CsvImportBatch> CreateAsync(CsvImportBatch batch);
        Task UpdateAsync(CsvImportBatch batch);
        Task DeleteAsync(CsvImportBatch batch);
        Task<bool> OrderNumberExistsForChannel(Guid salesChannelId, string orderNumber);
    }
}
