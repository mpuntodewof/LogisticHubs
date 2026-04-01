using Application.DTOs.Settings;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Settings
{
    public class SystemSettingUseCase
    {
        private readonly ISystemSettingRepository _repository;

        public SystemSettingUseCase(ISystemSettingRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SystemSettingDto>> GetAllAsync(string? group = null)
        {
            var settings = await _repository.GetAllAsync(group);
            return settings.Select(MapToDto).ToList();
        }

        public async Task<SystemSettingDto?> GetByKeyAsync(string key)
        {
            var setting = await _repository.GetByKeyAsync(key);
            return setting == null ? null : MapToDto(setting);
        }

        public async Task UpdateAsync(Guid id, UpdateSystemSettingRequest request)
        {
            var setting = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("System setting not found.");

            if (setting.IsReadOnly)
                throw new InvalidOperationException("Cannot update a read-only setting.");

            setting.Value = request.Value;

            await _repository.UpdateAsync(setting);
        }

        private static SystemSettingDto MapToDto(SystemSetting s) => new()
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
