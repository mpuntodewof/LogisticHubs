using Application.DTOs.Users;
using Application.Interfaces;

namespace Application.UseCases.Users
{
    public class UserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;

        public UserUseCase(IUserRepository userRepository, IAuthRepository authRepository)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _authRepository.GetUserRoleNamesAsync(user.Id);
                result.Add(new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Roles = roles
                });
            }

            return result;
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            var roles = await _authRepository.GetUserRoleNamesAsync(user.Id);
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = roles
            };
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"User {id} not found.");

            if (request.Name != null) user.Name = request.Name;
            if (request.IsActive.HasValue) user.IsActive = request.IsActive.Value;

            await _userRepository.UpdateAsync(user);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"User {id} not found.");

            await _userRepository.DeleteAsync(user);
        }

        // ── Roles ────────────────────────────────────────────────────────────────

        public async Task AssignRoleAsync(Guid userId, Guid roleId, Guid assignedBy)
        {
            _ = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            var role = await _userRepository.GetRoleByIdAsync(roleId)
                ?? throw new KeyNotFoundException($"Role {roleId} not found.");

            if (await _userRepository.UserHasRoleAsync(userId, roleId))
                throw new InvalidOperationException($"User already has role '{role.Name}'.");

            await _userRepository.AssignRoleAsync(userId, roleId, assignedBy);
        }

        public async Task RevokeRoleAsync(Guid userId, Guid roleId)
        {
            _ = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            if (!await _userRepository.UserHasRoleAsync(userId, roleId))
                throw new InvalidOperationException("User does not have this role.");

            await _userRepository.RevokeRoleAsync(userId, roleId);
        }
    }
}
