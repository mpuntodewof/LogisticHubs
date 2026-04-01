using Application.DTOs.Common;
using Application.DTOs.Purchase;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Purchase
{
    public class SupplierUseCase
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierUseCase(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<PagedResult<SupplierDto>> GetPagedAsync(PagedRequest request, bool? isActive = null)
        {
            var result = await _supplierRepository.GetPagedAsync(request, isActive);

            return new PagedResult<SupplierDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<SupplierDetailDto?> GetByIdAsync(Guid id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            return supplier == null ? null : MapToDetailDto(supplier);
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierRequest request)
        {
            var supplierCode = await GenerateSupplierCodeAsync();

            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                SupplierCode = supplierCode,
                CompanyName = request.CompanyName,
                ContactName = request.ContactName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                City = request.City,
                Province = request.Province,
                PostalCode = request.PostalCode,
                Country = request.Country ?? "Indonesia",
                TaxId = request.TaxId,
                PaymentTermId = request.PaymentTermId,
                BankName = request.BankName,
                BankAccountNumber = request.BankAccountNumber,
                BankAccountName = request.BankAccountName,
                Notes = request.Notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _supplierRepository.CreateAsync(supplier);
            return MapToDto(created);
        }

        public async Task UpdateAsync(Guid id, UpdateSupplierRequest request)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Supplier not found.");

            if (request.CompanyName != null) supplier.CompanyName = request.CompanyName;
            if (request.ContactName != null) supplier.ContactName = request.ContactName;
            if (request.Email != null) supplier.Email = request.Email;
            if (request.Phone != null) supplier.Phone = request.Phone;
            if (request.Address != null) supplier.Address = request.Address;
            if (request.City != null) supplier.City = request.City;
            if (request.Province != null) supplier.Province = request.Province;
            if (request.PostalCode != null) supplier.PostalCode = request.PostalCode;
            if (request.Country != null) supplier.Country = request.Country;
            if (request.TaxId != null) supplier.TaxId = request.TaxId;
            if (request.PaymentTermId.HasValue) supplier.PaymentTermId = request.PaymentTermId;
            if (request.BankName != null) supplier.BankName = request.BankName;
            if (request.BankAccountNumber != null) supplier.BankAccountNumber = request.BankAccountNumber;
            if (request.BankAccountName != null) supplier.BankAccountName = request.BankAccountName;
            if (request.Notes != null) supplier.Notes = request.Notes;
            if (request.IsActive.HasValue) supplier.IsActive = request.IsActive.Value;

            supplier.UpdatedAt = DateTime.UtcNow;

            await _supplierRepository.UpdateAsync(supplier);
        }

        public async Task DeleteAsync(Guid id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Supplier not found.");

            await _supplierRepository.DeleteAsync(supplier);
        }

        private async Task<string> GenerateSupplierCodeAsync()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string code;

            do
            {
                var suffix = new string(Enumerable.Range(0, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray());
                code = $"SUP-{suffix}";
            }
            while (await _supplierRepository.SupplierCodeExistsAsync(code));

            return code;
        }

        private static SupplierDto MapToDto(Supplier s) => new()
        {
            Id = s.Id,
            SupplierCode = s.SupplierCode,
            CompanyName = s.CompanyName,
            ContactName = s.ContactName,
            Email = s.Email,
            Phone = s.Phone,
            City = s.City,
            Province = s.Province,
            TaxId = s.TaxId,
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt
        };

        private static SupplierDetailDto MapToDetailDto(Supplier s) => new()
        {
            Id = s.Id,
            SupplierCode = s.SupplierCode,
            CompanyName = s.CompanyName,
            ContactName = s.ContactName,
            Email = s.Email,
            Phone = s.Phone,
            City = s.City,
            Province = s.Province,
            TaxId = s.TaxId,
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt,
            Address = s.Address,
            PostalCode = s.PostalCode,
            Country = s.Country,
            PaymentTermId = s.PaymentTermId,
            BankName = s.BankName,
            BankAccountNumber = s.BankAccountNumber,
            BankAccountName = s.BankAccountName,
            Notes = s.Notes
        };
    }
}
