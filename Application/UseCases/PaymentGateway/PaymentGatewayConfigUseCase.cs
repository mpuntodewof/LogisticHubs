using Application.DTOs.Common;
using Application.DTOs.PaymentGateway;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.PaymentGateway
{
    public class PaymentGatewayConfigUseCase
    {
        private readonly IPaymentGatewayConfigRepository _repository;

        public PaymentGatewayConfigUseCase(IPaymentGatewayConfigRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<PaymentGatewayConfigDto>> GetPagedAsync(PagedRequest request)
        {
            var result = await _repository.GetPagedAsync(request);

            return new PagedResult<PaymentGatewayConfigDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<PaymentGatewayConfigDto?> GetByIdAsync(Guid id)
        {
            var config = await _repository.GetByIdAsync(id);
            return config == null ? null : MapToDto(config);
        }

        public async Task<PaymentGatewayConfigDto> CreateAsync(CreatePaymentGatewayConfigRequest request)
        {
            if (!Enum.IsDefined(typeof(PaymentGatewayProvider), request.Provider))
                throw new InvalidOperationException($"Invalid payment gateway provider: {request.Provider}");

            var config = new PaymentGatewayConfig
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Provider = request.Provider.ToString(),
                MerchantId = request.MerchantId,
                ClientKey = request.ClientKey,
                ServerKey = request.ServerKey,
                WebhookSecret = request.WebhookSecret,
                BaseUrl = request.BaseUrl,
                IsActive = true,
                IsSandbox = request.IsSandbox,
                AdditionalConfig = request.AdditionalConfig,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(config);
            return MapToDto(created);
        }

        public async Task UpdateAsync(Guid id, UpdatePaymentGatewayConfigRequest request)
        {
            var config = await _repository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Payment gateway config not found.");

            if (request.Name != null) config.Name = request.Name;
            if (request.MerchantId != null) config.MerchantId = request.MerchantId;
            if (request.ClientKey != null) config.ClientKey = request.ClientKey;
            if (request.ServerKey != null) config.ServerKey = request.ServerKey;
            if (request.WebhookSecret != null) config.WebhookSecret = request.WebhookSecret;
            if (request.BaseUrl != null) config.BaseUrl = request.BaseUrl;
            if (request.IsActive.HasValue) config.IsActive = request.IsActive.Value;
            if (request.IsSandbox.HasValue) config.IsSandbox = request.IsSandbox.Value;
            if (request.AdditionalConfig != null) config.AdditionalConfig = request.AdditionalConfig;

            config.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(config);
        }

        public async Task DeleteAsync(Guid id)
        {
            var config = await _repository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Payment gateway config not found.");

            await _repository.DeleteAsync(config);
        }

        private static PaymentGatewayConfigDto MapToDto(PaymentGatewayConfig c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Provider = c.Provider,
            MerchantId = c.MerchantId,
            IsActive = c.IsActive,
            IsSandbox = c.IsSandbox,
            CreatedAt = c.CreatedAt
        };
    }
}
