using Application.DTOs.Common;
using Application.DTOs.Finance;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Finance
{
    public class PaymentTermUseCase
    {
        private readonly IPaymentTermRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentTermUseCase(IPaymentTermRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<PaymentTermDto>> GetPagedAsync(PagedRequest request)
        {
            var paged = await _repository.GetPagedAsync(request);
            return new PagedResult<PaymentTermDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<PaymentTermDto?> GetByIdAsync(Guid id)
        {
            var term = await _repository.GetByIdAsync(id);
            return term == null ? null : MapToDto(term);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<PaymentTermDto> CreateAsync(CreatePaymentTermRequest request)
        {
            if (await _repository.CodeExistsAsync(request.Code))
                throw new InvalidOperationException($"Payment term code '{request.Code}' already exists.");

            var term = new PaymentTerm
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                DueDays = request.DueDays,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(term);
            await _unitOfWork.SaveChangesAsync();
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdatePaymentTermRequest request)
        {
            var term = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Payment term {id} not found.");

            if (request.Name != null) term.Name = request.Name;
            if (request.Description != null) term.Description = request.Description;
            if (request.DueDays.HasValue) term.DueDays = request.DueDays.Value;
            if (request.IsActive.HasValue) term.IsActive = request.IsActive.Value;

            await _repository.UpdateAsync(term);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var term = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Payment term {id} not found.");

            await _repository.DeleteAsync(term);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static PaymentTermDto MapToDto(PaymentTerm p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code,
            Description = p.Description,
            DueDays = p.DueDays,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt
        };
    }
}
