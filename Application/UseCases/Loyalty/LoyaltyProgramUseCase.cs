using Application.DTOs.Common;
using Application.DTOs.Loyalty;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Loyalty
{
    public class LoyaltyProgramUseCase
    {
        private readonly ILoyaltyProgramRepository _programRepository;
        private readonly ILoyaltyTierRepository _tierRepository;

        public LoyaltyProgramUseCase(
            ILoyaltyProgramRepository programRepository,
            ILoyaltyTierRepository tierRepository)
        {
            _programRepository = programRepository;
            _tierRepository = tierRepository;
        }

        public async Task<PagedResult<LoyaltyProgramDto>> GetPagedAsync(PagedRequest request)
        {
            var result = await _programRepository.GetPagedAsync(request);

            return new PagedResult<LoyaltyProgramDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<LoyaltyProgramDetailDto?> GetByIdAsync(Guid id)
        {
            var program = await _programRepository.GetDetailByIdAsync(id);
            return program == null ? null : MapToDetailDto(program);
        }

        public async Task<LoyaltyProgramDto> CreateAsync(CreateLoyaltyProgramRequest request)
        {
            var program = new LoyaltyProgram
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                PointsPerIdrSpent = request.PointsPerIdrSpent,
                RedemptionRateIdr = request.RedemptionRateIdr,
                MinRedemptionPoints = request.MinRedemptionPoints ?? 100,
                PointExpiryDays = request.PointExpiryDays,
                IsActive = true,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            var created = await _programRepository.CreateAsync(program);
            return MapToDto(created);
        }

        public async Task UpdateAsync(Guid id, UpdateLoyaltyProgramRequest request)
        {
            var program = await _programRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Loyalty program not found.");

            if (request.Name != null) program.Name = request.Name;
            if (request.Description != null) program.Description = request.Description;
            if (request.PointsPerIdrSpent.HasValue) program.PointsPerIdrSpent = request.PointsPerIdrSpent.Value;
            if (request.RedemptionRateIdr.HasValue) program.RedemptionRateIdr = request.RedemptionRateIdr.Value;
            if (request.MinRedemptionPoints.HasValue) program.MinRedemptionPoints = request.MinRedemptionPoints.Value;
            if (request.PointExpiryDays.HasValue) program.PointExpiryDays = request.PointExpiryDays.Value;
            if (request.IsActive.HasValue) program.IsActive = request.IsActive.Value;
            if (request.Status.HasValue) program.Status = request.Status.Value.ToString();

            program.UpdatedAt = DateTime.UtcNow;

            await _programRepository.UpdateAsync(program);
        }

        public async Task DeleteAsync(Guid id)
        {
            var program = await _programRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Loyalty program not found.");

            await _programRepository.DeleteAsync(program);
        }

        // ── Tier Management ────────────────────────────────────────────────

        public async Task<IEnumerable<LoyaltyTierDto>> GetTiersByProgramAsync(Guid programId)
        {
            var tiers = await _tierRepository.GetByProgramIdAsync(programId);
            return tiers.Select(MapTierToDto);
        }

        public async Task<LoyaltyTierDto> CreateTierAsync(Guid programId, CreateLoyaltyTierRequest request)
        {
            var program = await _programRepository.GetByIdAsync(programId)
                ?? throw new InvalidOperationException("Loyalty program not found.");

            var tier = new LoyaltyTier
            {
                Id = Guid.NewGuid(),
                LoyaltyProgramId = programId,
                Name = request.Name,
                MinPointsThreshold = request.MinPointsThreshold,
                PointsMultiplier = request.PointsMultiplier ?? 1.0m,
                DiscountPercentage = request.DiscountPercentage ?? 0m,
                Description = request.Description,
                SortOrder = request.SortOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _tierRepository.CreateAsync(tier);
            return MapTierToDto(created);
        }

        public async Task UpdateTierAsync(Guid programId, Guid tierId, UpdateLoyaltyTierRequest request)
        {
            var tier = await _tierRepository.GetByIdAsync(tierId)
                ?? throw new InvalidOperationException("Loyalty tier not found.");

            if (tier.LoyaltyProgramId != programId)
                throw new InvalidOperationException("Tier does not belong to this program.");

            if (request.Name != null) tier.Name = request.Name;
            if (request.MinPointsThreshold.HasValue) tier.MinPointsThreshold = request.MinPointsThreshold.Value;
            if (request.PointsMultiplier.HasValue) tier.PointsMultiplier = request.PointsMultiplier.Value;
            if (request.DiscountPercentage.HasValue) tier.DiscountPercentage = request.DiscountPercentage.Value;
            if (request.Description != null) tier.Description = request.Description;
            if (request.SortOrder.HasValue) tier.SortOrder = request.SortOrder.Value;
            if (request.IsActive.HasValue) tier.IsActive = request.IsActive.Value;

            tier.UpdatedAt = DateTime.UtcNow;

            await _tierRepository.UpdateAsync(tier);
        }

        public async Task DeleteTierAsync(Guid programId, Guid tierId)
        {
            var tier = await _tierRepository.GetByIdAsync(tierId)
                ?? throw new InvalidOperationException("Loyalty tier not found.");

            if (tier.LoyaltyProgramId != programId)
                throw new InvalidOperationException("Tier does not belong to this program.");

            await _tierRepository.DeleteAsync(tier);
        }

        // ── Mapping ────────────────────────────────────────────────────────

        private static LoyaltyProgramDto MapToDto(LoyaltyProgram p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            PointsPerIdrSpent = p.PointsPerIdrSpent,
            RedemptionRateIdr = p.RedemptionRateIdr,
            MinRedemptionPoints = p.MinRedemptionPoints,
            PointExpiryDays = p.PointExpiryDays,
            IsActive = p.IsActive,
            Status = p.Status,
            CreatedAt = p.CreatedAt
        };

        private static LoyaltyProgramDetailDto MapToDetailDto(LoyaltyProgram p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            PointsPerIdrSpent = p.PointsPerIdrSpent,
            RedemptionRateIdr = p.RedemptionRateIdr,
            MinRedemptionPoints = p.MinRedemptionPoints,
            PointExpiryDays = p.PointExpiryDays,
            IsActive = p.IsActive,
            Status = p.Status,
            CreatedAt = p.CreatedAt,
            Tiers = p.Tiers?.Select(MapTierToDto).ToList() ?? new List<LoyaltyTierDto>()
        };

        private static LoyaltyTierDto MapTierToDto(LoyaltyTier t) => new()
        {
            Id = t.Id,
            LoyaltyProgramId = t.LoyaltyProgramId,
            Name = t.Name,
            MinPointsThreshold = t.MinPointsThreshold,
            PointsMultiplier = t.PointsMultiplier,
            DiscountPercentage = t.DiscountPercentage,
            Description = t.Description,
            SortOrder = t.SortOrder,
            IsActive = t.IsActive
        };
    }
}
