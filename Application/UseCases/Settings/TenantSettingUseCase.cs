using Application.DTOs.Settings;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Settings
{
    public class TenantSettingUseCase
    {
        private readonly ITenantSettingRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public TenantSettingUseCase(ITenantSettingRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TenantSettingDto>> GetAllAsync(string? group = null)
        {
            var settings = await _repository.GetAllAsync(group);
            return settings.Select(MapToDto).ToList();
        }

        public async Task<TenantSettingDto?> GetByKeyAsync(string key)
        {
            var setting = await _repository.GetByKeyAsync(key);
            return setting == null ? null : MapToDto(setting);
        }

        public async Task UpdateAsync(Guid id, UpdateTenantSettingRequest request)
        {
            var setting = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Tenant setting not found.");

            if (setting.IsReadOnly)
                throw new InvalidOperationException("Cannot update a read-only setting.");

            setting.Value = request.Value;

            await _repository.UpdateAsync(setting);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task BulkUpdateAsync(BulkUpdateTenantSettingsRequest request)
        {
            foreach (var kvp in request.Settings)
            {
                var setting = await _repository.GetByKeyAsync(kvp.Key);
                if (setting == null || setting.IsReadOnly)
                    continue;

                setting.Value = kvp.Value;
                await _repository.UpdateAsync(setting);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private static TenantSettingDto MapToDto(TenantSetting s) => new()
        {
            Id = s.Id,
            Key = s.Key,
            Value = s.Value,
            Description = s.Description,
            Group = s.Group,
            ValueType = s.ValueType,
            IsReadOnly = s.IsReadOnly
        };
    }
}
