using System.Security.Cryptography;
using System.Text;
using Application.DTOs.Common;
using Application.DTOs.Settings;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Settings
{
    public class ApiKeyUseCase
    {
        private readonly IApiKeyRepository _repository;

        public ApiKeyUseCase(IApiKeyRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<ApiKeyDto>> GetPagedAsync(PagedRequest request, bool? isActive = null)
        {
            var result = await _repository.GetPagedAsync(request, isActive);
            return new PagedResult<ApiKeyDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<ApiKeyDetailDto?> GetByIdAsync(Guid id)
        {
            var apiKey = await _repository.GetByIdAsync(id);
            return apiKey == null ? null : MapToDetailDto(apiKey);
        }

        public async Task<CreateApiKeyResponse> CreateAsync(CreateApiKeyRequest request)
        {
            var plainTextKey = "nk_live_" + GenerateRandomHex(32);
            var keyHash = ComputeSha256Hash(plainTextKey);
            var keyPrefix = plainTextKey[..12];

            var apiKey = new ApiKey
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                KeyHash = keyHash,
                KeyPrefix = keyPrefix,
                PermissionsJson = request.PermissionsJson,
                ExpiresAt = request.ExpiresAt,
                RateLimitPerMinute = request.RateLimitPerMinute ?? 60,
                RateLimitPerDay = request.RateLimitPerDay ?? 10000,
                IsActive = true
            };

            await _repository.CreateAsync(apiKey);

            return new CreateApiKeyResponse
            {
                Id = apiKey.Id,
                Name = apiKey.Name,
                KeyPrefix = apiKey.KeyPrefix,
                PlainTextKey = plainTextKey
            };
        }

        public async Task UpdateAsync(Guid id, UpdateApiKeyRequest request)
        {
            var apiKey = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("API key not found.");

            if (request.Name != null) apiKey.Name = request.Name;
            if (request.PermissionsJson != null) apiKey.PermissionsJson = request.PermissionsJson;
            if (request.ExpiresAt.HasValue) apiKey.ExpiresAt = request.ExpiresAt;
            if (request.RateLimitPerMinute.HasValue) apiKey.RateLimitPerMinute = request.RateLimitPerMinute.Value;
            if (request.RateLimitPerDay.HasValue) apiKey.RateLimitPerDay = request.RateLimitPerDay.Value;
            if (request.IsActive.HasValue) apiKey.IsActive = request.IsActive.Value;

            await _repository.UpdateAsync(apiKey);
        }

        public async Task DeleteAsync(Guid id)
        {
            var apiKey = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("API key not found.");

            await _repository.DeleteAsync(apiKey);
        }

        public async Task<CreateApiKeyResponse> RegenerateAsync(Guid id)
        {
            var apiKey = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("API key not found.");

            var plainTextKey = "nk_live_" + GenerateRandomHex(32);
            var keyHash = ComputeSha256Hash(plainTextKey);
            var keyPrefix = plainTextKey[..12];

            apiKey.KeyHash = keyHash;
            apiKey.KeyPrefix = keyPrefix;

            await _repository.UpdateAsync(apiKey);

            return new CreateApiKeyResponse
            {
                Id = apiKey.Id,
                Name = apiKey.Name,
                KeyPrefix = apiKey.KeyPrefix,
                PlainTextKey = plainTextKey
            };
        }

        private static string GenerateRandomHex(int byteCount)
        {
            var bytes = RandomNumberGenerator.GetBytes(byteCount);
            return Convert.ToHexString(bytes).ToLower();
        }

        private static string ComputeSha256Hash(string rawData)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToHexString(bytes).ToLower();
        }

        private static ApiKeyDto MapToDto(ApiKey k) => new()
        {
            Id = k.Id,
            Name = k.Name,
            KeyPrefix = k.KeyPrefix,
            ExpiresAt = k.ExpiresAt,
            RateLimitPerMinute = k.RateLimitPerMinute,
            RateLimitPerDay = k.RateLimitPerDay,
            IsActive = k.IsActive,
            LastUsedAt = k.LastUsedAt,
            CreatedAt = k.CreatedAt
        };

        private static ApiKeyDetailDto MapToDetailDto(ApiKey k) => new()
        {
            Id = k.Id,
            Name = k.Name,
            KeyPrefix = k.KeyPrefix,
            ExpiresAt = k.ExpiresAt,
            RateLimitPerMinute = k.RateLimitPerMinute,
            RateLimitPerDay = k.RateLimitPerDay,
            IsActive = k.IsActive,
            LastUsedAt = k.LastUsedAt,
            CreatedAt = k.CreatedAt,
            PermissionsJson = k.PermissionsJson,
            LastUsedIp = k.LastUsedIp
        };
    }
}
