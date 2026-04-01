using Application.DTOs.Notifications;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Notifications
{
    public class NotificationPreferenceUseCase
    {
        private readonly INotificationPreferenceRepository _preferenceRepository;
        private readonly ICurrentUserService _currentUserService;

        public NotificationPreferenceUseCase(INotificationPreferenceRepository preferenceRepository, ICurrentUserService currentUserService)
        {
            _preferenceRepository = preferenceRepository;
            _currentUserService = currentUserService;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<NotificationPreferenceDto> GetMyPreferencesAsync()
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var preference = await _preferenceRepository.GetByUserIdAsync(userId);

            if (preference == null)
            {
                preference = new NotificationPreference
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    EnableEmail = true,
                    EnableSMS = false,
                    EnablePush = true,
                    CreatedAt = DateTime.UtcNow
                };

                preference = await _preferenceRepository.CreateAsync(preference);
            }

            return MapToDto(preference);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task<NotificationPreferenceDto> UpdateMyPreferencesAsync(UpdateNotificationPreferenceRequest request)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var preference = await _preferenceRepository.GetByUserIdAsync(userId);

            if (preference == null)
            {
                preference = new NotificationPreference
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    EnableEmail = request.EnableEmail ?? true,
                    EnableSMS = request.EnableSMS ?? false,
                    EnablePush = request.EnablePush ?? true,
                    CreatedAt = DateTime.UtcNow
                };

                preference = await _preferenceRepository.CreateAsync(preference);
            }
            else
            {
                if (request.EnableEmail.HasValue) preference.EnableEmail = request.EnableEmail.Value;
                if (request.EnableSMS.HasValue) preference.EnableSMS = request.EnableSMS.Value;
                if (request.EnablePush.HasValue) preference.EnablePush = request.EnablePush.Value;

                await _preferenceRepository.UpdateAsync(preference);
            }

            return MapToDto(preference);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static NotificationPreferenceDto MapToDto(NotificationPreference p) => new()
        {
            Id = p.Id,
            UserId = p.UserId,
            EnableEmail = p.EnableEmail,
            EnableSMS = p.EnableSMS,
            EnablePush = p.EnablePush
        };
    }
}
