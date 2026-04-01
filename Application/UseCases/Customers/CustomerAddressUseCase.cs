using Application.DTOs.Customers;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Customers
{
    public class CustomerAddressUseCase
    {
        private readonly ICustomerAddressRepository _addressRepository;

        public CustomerAddressUseCase(ICustomerAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<CustomerAddressDto>> GetByCustomerIdAsync(Guid customerId)
        {
            var addresses = await _addressRepository.GetByCustomerIdAsync(customerId);
            return addresses.Select(MapToDto);
        }

        public async Task<CustomerAddressDto?> GetByIdAsync(Guid id)
        {
            var address = await _addressRepository.GetByIdAsync(id);
            return address == null ? null : MapToDto(address);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<CustomerAddressDto> CreateAsync(Guid customerId, CreateCustomerAddressRequest request)
        {
            if (request.IsDefault)
            {
                await _addressRepository.ClearDefaultsAsync(customerId);
            }

            var address = new CustomerAddress
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Label = request.Label,
                AddressType = request.AddressType.ToString(),
                RecipientName = request.RecipientName,
                Phone = request.Phone,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                Province = request.Province,
                PostalCode = request.PostalCode,
                Country = request.Country,
                IsDefault = request.IsDefault,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _addressRepository.CreateAsync(address);
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateCustomerAddressRequest request)
        {
            var address = await _addressRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Customer address {id} not found.");

            if (request.Label != null) address.Label = request.Label;
            if (request.AddressType.HasValue) address.AddressType = request.AddressType.Value.ToString();
            if (request.RecipientName != null) address.RecipientName = request.RecipientName;
            if (request.Phone != null) address.Phone = request.Phone;
            if (request.AddressLine1 != null) address.AddressLine1 = request.AddressLine1;
            if (request.AddressLine2 != null) address.AddressLine2 = request.AddressLine2;
            if (request.City != null) address.City = request.City;
            if (request.Province != null) address.Province = request.Province;
            if (request.PostalCode != null) address.PostalCode = request.PostalCode;
            if (request.Country != null) address.Country = request.Country;

            if (request.IsDefault.HasValue)
            {
                if (request.IsDefault.Value)
                {
                    await _addressRepository.ClearDefaultsAsync(address.CustomerId);
                }
                address.IsDefault = request.IsDefault.Value;
            }

            await _addressRepository.UpdateAsync(address);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var address = await _addressRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Customer address {id} not found.");

            await _addressRepository.DeleteAsync(address);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static CustomerAddressDto MapToDto(CustomerAddress a) => new()
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
