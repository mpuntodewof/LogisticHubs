using Application.DTOs.Branches;
using Application.DTOs.Common;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Branches
{
    public class BranchUseCase
    {
        private readonly IBranchRepository _branchRepository;

        public BranchUseCase(IBranchRepository branchRepository)
        {
            _branchRepository = branchRepository;
        }

        // ── Get ──────────────────────────────────────────────────────────────────

        public async Task<PagedResult<BranchDto>> GetPagedAsync(PagedRequest request)
        {
            var paged = await _branchRepository.GetPagedAsync(request);
            return new PagedResult<BranchDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<BranchDto?> GetByIdAsync(Guid id)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            return branch == null ? null : MapToDto(branch);
        }

        // ── Create ───────────────────────────────────────────────────────────────

        public async Task<BranchDto> CreateAsync(CreateBranchRequest request)
        {
            if (await _branchRepository.CodeExistsAsync(request.Code))
                throw new InvalidOperationException($"A branch with code '{request.Code}' already exists.");

            var branch = new Branch
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = request.Code,
                Address = request.Address,
                City = request.City,
                Province = request.Province,
                PostalCode = request.PostalCode,
                Phone = request.Phone,
                WarehouseId = request.WarehouseId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _branchRepository.CreateAsync(branch);
            return MapToDto(created);
        }

        // ── Update ───────────────────────────────────────────────────────────────

        public async Task UpdateAsync(Guid id, UpdateBranchRequest request)
        {
            var branch = await _branchRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Branch {id} not found.");

            if (request.Name != null) branch.Name = request.Name;
            if (request.Code != null) branch.Code = request.Code;
            if (request.Address != null) branch.Address = request.Address;
            if (request.City != null) branch.City = request.City;
            if (request.Province != null) branch.Province = request.Province;
            if (request.PostalCode != null) branch.PostalCode = request.PostalCode;
            if (request.Phone != null) branch.Phone = request.Phone;
            if (request.WarehouseId.HasValue) branch.WarehouseId = request.WarehouseId;
            if (request.IsActive.HasValue) branch.IsActive = request.IsActive.Value;

            await _branchRepository.UpdateAsync(branch);
        }

        // ── Delete ───────────────────────────────────────────────────────────────

        public async Task DeleteAsync(Guid id)
        {
            var branch = await _branchRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Branch {id} not found.");

            await _branchRepository.DeleteAsync(branch);
        }

        // ── Branch Users ─────────────────────────────────────────────────────────

        public async Task<IEnumerable<BranchUserDto>> GetBranchUsersAsync(Guid branchId)
        {
            var users = await _branchRepository.GetBranchUsersAsync(branchId);
            return users.Select(MapBranchUserToDto);
        }

        public async Task AssignUserAsync(Guid branchId, AssignBranchUserRequest request)
        {
            var branchUser = new BranchUser
            {
                BranchId = branchId,
                UserId = request.UserId,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _branchRepository.AssignUserAsync(branchUser);
        }

        public async Task RemoveUserAsync(Guid branchId, Guid userId)
        {
            await _branchRepository.RemoveUserAsync(branchId, userId);
        }

        public async Task<IEnumerable<BranchUserDto>> GetBranchesByUserIdAsync(Guid userId)
        {
            var branchUsers = await _branchRepository.GetBranchesByUserIdAsync(userId);
            return branchUsers.Select(MapBranchUserToDto);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static BranchDto MapToDto(Branch b) => new()
        {
            Id = b.Id,
            Name = b.Name,
            Code = b.Code,
            Address = b.Address,
            City = b.City,
            Province = b.Province,
            PostalCode = b.PostalCode,
            Phone = b.Phone,
            WarehouseId = b.WarehouseId,
            WarehouseName = b.Warehouse?.Name,
            IsActive = b.IsActive,
            CreatedAt = b.CreatedAt
        };

        private static BranchUserDto MapBranchUserToDto(BranchUser bu) => new()
        {
            BranchId = bu.BranchId,
            BranchName = bu.Branch?.Name ?? string.Empty,
            UserId = bu.UserId,
            UserName = bu.User?.Name ?? string.Empty,
            UserEmail = bu.User?.Email ?? string.Empty,
            AssignedAt = bu.AssignedAt,
            IsActive = bu.IsActive
        };
    }
}
