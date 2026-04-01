using Application.DTOs.Common;
using Application.DTOs.Tax;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Tax
{
    public class TaxRateUseCase
    {
        private readonly ITaxRateRepository _taxRateRepository;

        public TaxRateUseCase(ITaxRateRepository taxRateRepository)
        {
            _taxRateRepository = taxRateRepository;
        }

        public async Task<PagedResult<TaxRateDto>> GetPagedAsync(PagedRequest request)
        {
            var result = await _taxRateRepository.GetPagedAsync(request);

            return new PagedResult<TaxRateDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<TaxRateDto?> GetByIdAsync(Guid id)
        {
            var taxRate = await _taxRateRepository.GetByIdAsync(id);
            return taxRate == null ? null : MapToDto(taxRate);
        }

        public async Task<TaxRateDto> CreateAsync(CreateTaxRateRequest request)
        {
            if (await _taxRateRepository.CodeExistsAsync(request.Code))
                throw new InvalidOperationException($"Tax rate with code '{request.Code}' already exists.");

            var taxRate = new TaxRate
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = request.Code,
                TaxType = request.TaxType.ToString(),
                Rate = request.Rate,
                Description = request.Description,
                IsActive = true,
                EffectiveFrom = request.EffectiveFrom,
                EffectiveTo = request.EffectiveTo,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _taxRateRepository.CreateAsync(taxRate);
            return MapToDto(created);
        }

        public async Task UpdateAsync(Guid id, UpdateTaxRateRequest request)
        {
            var taxRate = await _taxRateRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Tax rate not found.");

            if (request.Name != null) taxRate.Name = request.Name;
            if (request.Description != null) taxRate.Description = request.Description;
            if (request.Rate.HasValue) taxRate.Rate = request.Rate.Value;
            if (request.IsActive.HasValue) taxRate.IsActive = request.IsActive.Value;
            if (request.EffectiveTo.HasValue) taxRate.EffectiveTo = request.EffectiveTo.Value;

            taxRate.UpdatedAt = DateTime.UtcNow;

            await _taxRateRepository.UpdateAsync(taxRate);
        }

        public async Task DeleteAsync(Guid id)
        {
            var taxRate = await _taxRateRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Tax rate not found.");

            await _taxRateRepository.DeleteAsync(taxRate);
        }

        public async Task AssignToProductAsync(Guid taxRateId, Guid productId)
        {
            var taxRate = await _taxRateRepository.GetByIdAsync(taxRateId)
                ?? throw new InvalidOperationException("Tax rate not found.");

            await _taxRateRepository.AssignToProductAsync(productId, taxRateId, taxRate.TenantId);
        }

        public async Task RemoveFromProductAsync(Guid taxRateId, Guid productId)
        {
            _ = await _taxRateRepository.GetByIdAsync(taxRateId)
                ?? throw new InvalidOperationException("Tax rate not found.");

            await _taxRateRepository.RemoveFromProductAsync(productId, taxRateId);
        }

        public async Task<IEnumerable<TaxRateDto>> GetByProductIdAsync(Guid productId)
        {
            var taxRates = await _taxRateRepository.GetActiveByProductIdAsync(productId);
            return taxRates.Select(MapToDto);
        }

        private static TaxRateDto MapToDto(TaxRate t) => new()
        {
            Id = t.Id,
            Name = t.Name,
            Code = t.Code,
            TaxType = t.TaxType,
            Rate = t.Rate,
            Description = t.Description,
            IsActive = t.IsActive,
            EffectiveFrom = t.EffectiveFrom,
            EffectiveTo = t.EffectiveTo,
            CreatedAt = t.CreatedAt
        };
    }
}
