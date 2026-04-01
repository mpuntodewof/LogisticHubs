using Application.DTOs.Common;
using Application.DTOs.Customers;
using Application.Helpers;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Customers
{
    public class CustomerGroupUseCase
    {
        private readonly ICustomerGroupRepository _customerGroupRepository;

        public CustomerGroupUseCase(ICustomerGroupRepository customerGroupRepository)
        {
            _customerGroupRepository = customerGroupRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<CustomerGroupDto>> GetPagedAsync(PagedRequest request)
        {
            var paged = await _customerGroupRepository.GetPagedAsync(request);
            return new PagedResult<CustomerGroupDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<CustomerGroupDto?> GetByIdAsync(Guid id)
        {
            var customerGroup = await _customerGroupRepository.GetByIdAsync(id);
            return customerGroup == null ? null : MapToDto(customerGroup);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<CustomerGroupDto> CreateAsync(CreateCustomerGroupRequest request)
        {
            var slug = SlugHelper.Generate(request.Name);

            if (await _customerGroupRepository.SlugExistsAsync(slug))
                throw new InvalidOperationException($"A customer group with slug '{slug}' already exists.");

            var customerGroup = new CustomerGroup
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = slug,
                Description = request.Description,
                DiscountPercentage = request.DiscountPercentage,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _customerGroupRepository.CreateAsync(customerGroup);
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateCustomerGroupRequest request)
        {
            var customerGroup = await _customerGroupRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Customer group {id} not found.");

            if (request.Name != null)
            {
                customerGroup.Name = request.Name;
                customerGroup.Slug = SlugHelper.Generate(request.Name);
            }

            if (request.Description != null) customerGroup.Description = request.Description;
            if (request.DiscountPercentage.HasValue) customerGroup.DiscountPercentage = request.DiscountPercentage.Value;
            if (request.IsActive.HasValue) customerGroup.IsActive = request.IsActive.Value;

            await _customerGroupRepository.UpdateAsync(customerGroup);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var customerGroup = await _customerGroupRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Customer group {id} not found.");

            await _customerGroupRepository.DeleteAsync(customerGroup);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static CustomerGroupDto MapToDto(CustomerGroup cg) => new()
        {
            Id = cg.Id,
            Name = cg.Name,
            Slug = cg.Slug,
            Description = cg.Description,
            DiscountPercentage = cg.DiscountPercentage,
            IsActive = cg.IsActive,
            CreatedAt = cg.CreatedAt
        };
    }
}
