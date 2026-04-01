using Application.DTOs.Common;
using Application.DTOs.Customers;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Customers
{
    public class CustomerUseCase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerUseCase(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<CustomerDto>> GetPagedAsync(PagedRequest request)
        {
            var paged = await _customerRepository.GetPagedAsync(request);
            return new PagedResult<CustomerDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<CustomerDetailDto?> GetByIdAsync(Guid id)
        {
            var customer = await _customerRepository.GetDetailByIdAsync(id);
            return customer == null ? null : MapToDetailDto(customer);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request)
        {
            var customerCode = await GenerateCustomerCodeAsync();

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                CustomerCode = customerCode,
                CustomerType = request.CustomerType.ToString(),
                Name = request.Name,
                CompanyName = request.CompanyName,
                Email = request.Email,
                Phone = request.Phone,
                TaxId = request.TaxId,
                Notes = request.Notes,
                CustomerGroupId = request.CustomerGroupId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _customerRepository.CreateAsync(customer);
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateCustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Customer {id} not found.");

            if (request.CustomerType.HasValue) customer.CustomerType = request.CustomerType.Value.ToString();
            if (request.Name != null) customer.Name = request.Name;
            if (request.CompanyName != null) customer.CompanyName = request.CompanyName;
            if (request.Email != null) customer.Email = request.Email;
            if (request.Phone != null) customer.Phone = request.Phone;
            if (request.TaxId != null) customer.TaxId = request.TaxId;
            if (request.Notes != null) customer.Notes = request.Notes;
            if (request.CustomerGroupId.HasValue) customer.CustomerGroupId = request.CustomerGroupId;
            if (request.IsActive.HasValue) customer.IsActive = request.IsActive.Value;

            await _customerRepository.UpdateAsync(customer);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Customer {id} not found.");

            await _customerRepository.DeleteAsync(customer);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private async Task<string> GenerateCustomerCodeAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string code;

            do
            {
                code = "CUST-" + new string(Enumerable.Range(0, 8)
                    .Select(_ => chars[random.Next(chars.Length)]).ToArray());
            }
            while (await _customerRepository.CustomerCodeExistsAsync(code));

            return code;
        }

        private static CustomerDto MapToDto(Customer c) => new()
        {
            Id = c.Id,
            CustomerCode = c.CustomerCode,
            CustomerType = c.CustomerType,
            Name = c.Name,
            CompanyName = c.CompanyName,
            Email = c.Email,
            Phone = c.Phone,
            TaxId = c.TaxId,
            CustomerGroupId = c.CustomerGroupId,
            CustomerGroupName = c.CustomerGroup?.Name,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        };

        private static CustomerDetailDto MapToDetailDto(Customer c) => new()
        {
            Id = c.Id,
            CustomerCode = c.CustomerCode,
            CustomerType = c.CustomerType,
            Name = c.Name,
            CompanyName = c.CompanyName,
            Email = c.Email,
            Phone = c.Phone,
            TaxId = c.TaxId,
            CustomerGroupId = c.CustomerGroupId,
            CustomerGroupName = c.CustomerGroup?.Name,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            Addresses = c.Addresses.Select(MapAddressToDto).ToList()
        };

        private static CustomerAddressDto MapAddressToDto(CustomerAddress a) => new()
        {
            Id = a.Id,
            CustomerId = a.CustomerId,
            Label = a.Label,
            AddressType = a.AddressType,
            RecipientName = a.RecipientName,
            Phone = a.Phone,
            AddressLine1 = a.AddressLine1,
            AddressLine2 = a.AddressLine2,
            City = a.City,
            Province = a.Province,
            PostalCode = a.PostalCode,
            Country = a.Country,
            IsDefault = a.IsDefault,
            CreatedAt = a.CreatedAt
        };
    }
}
