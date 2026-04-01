using Application.DTOs.Common;
using Application.DTOs.Loyalty;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases.Loyalty
{
    public class LoyaltyMembershipUseCase
    {
        private readonly ILoyaltyMembershipRepository _membershipRepository;
        private readonly ILoyaltyProgramRepository _programRepository;
        private readonly ILoyaltyTierRepository _tierRepository;
        private readonly ILoyaltyPointTransactionRepository _transactionRepository;

        public LoyaltyMembershipUseCase(
            ILoyaltyMembershipRepository membershipRepository,
            ILoyaltyProgramRepository programRepository,
            ILoyaltyTierRepository tierRepository,
            ILoyaltyPointTransactionRepository transactionRepository)
        {
            _membershipRepository = membershipRepository;
            _programRepository = programRepository;
            _tierRepository = tierRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<PagedResult<LoyaltyMembershipDto>> GetPagedAsync(
            PagedRequest request, Guid? programId = null, Guid? customerId = null)
        {
            var result = await _membershipRepository.GetPagedAsync(request, programId, customerId);

            return new PagedResult<LoyaltyMembershipDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<LoyaltyMembershipDto?> GetByIdAsync(Guid id)
        {
            var membership = await _membershipRepository.GetByIdAsync(id);
            return membership == null ? null : MapToDto(membership);
        }

        public async Task<LoyaltyMembershipDto> EnrollAsync(EnrollCustomerRequest request)
        {
            var program = await _programRepository.GetByIdAsync(request.LoyaltyProgramId)
                ?? throw new InvalidOperationException("Loyalty program not found.");

            if (!program.IsActive || program.Status != "Active")
                throw new InvalidOperationException("Loyalty program is not active.");

            var existing = await _membershipRepository.GetByCustomerAndProgramAsync(request.CustomerId, request.LoyaltyProgramId);
            if (existing != null)
                throw new InvalidOperationException("Customer is already enrolled in this loyalty program.");

            var membership = new LoyaltyMembership
            {
                Id = Guid.NewGuid(),
                LoyaltyProgramId = request.LoyaltyProgramId,
                CustomerId = request.CustomerId,
                AvailablePoints = 0,
                LifetimePoints = 0,
                TotalRedeemed = 0,
                JoinedAt = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _membershipRepository.CreateAsync(membership);

            // Reload with includes
            var loaded = await _membershipRepository.GetByIdAsync(created.Id);
            return MapToDto(loaded!);
        }

        public async Task<LoyaltyPointTransactionDto> EarnPointsAsync(
            Guid membershipId, decimal orderAmount, string? referenceNumber = null, Guid? referenceId = null)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId)
                ?? throw new InvalidOperationException("Loyalty membership not found.");

            var program = await _programRepository.GetByIdAsync(membership.LoyaltyProgramId)
                ?? throw new InvalidOperationException("Loyalty program not found.");

            var tier = membership.CurrentTierId.HasValue
                ? await _tierRepository.GetByIdAsync(membership.CurrentTierId.Value)
                : null;

            var multiplier = tier?.PointsMultiplier ?? 1.0m;
            var points = (int)Math.Round(orderAmount * program.PointsPerIdrSpent * multiplier);

            if (points <= 0) return MapTransactionToDto(new LoyaltyPointTransaction { Points = 0 });

            var balanceBefore = membership.AvailablePoints;

            var transaction = new LoyaltyPointTransaction
            {
                Id = Guid.NewGuid(),
                LoyaltyMembershipId = membershipId,
                TransactionType = "Earn",
                Points = points,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceBefore + points,
                Description = $"Points earned from order {referenceNumber ?? "N/A"}",
                ReferenceDocumentType = "SalesOrder",
                ReferenceDocumentId = referenceId,
                ReferenceDocumentNumber = referenceNumber,
                ExpiresAt = program.PointExpiryDays.HasValue ? DateTime.UtcNow.AddDays(program.PointExpiryDays.Value) : null,
                TransactionDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepository.CreateAsync(transaction);

            membership.AvailablePoints += points;
            membership.LifetimePoints += points;
            membership.LastActivityAt = DateTime.UtcNow;
            membership.UpdatedAt = DateTime.UtcNow;

            await _membershipRepository.UpdateAsync(membership);

            // Check tier upgrade
            await RecalculateTierAsync(membershipId);

            return MapTransactionToDto(transaction);
        }

        public async Task<LoyaltyPointTransactionDto> RedeemPointsAsync(Guid membershipId, RedeemPointsRequest request)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId)
                ?? throw new InvalidOperationException("Loyalty membership not found.");

            var program = await _programRepository.GetByIdAsync(membership.LoyaltyProgramId)
                ?? throw new InvalidOperationException("Loyalty program not found.");

            if (request.Points < program.MinRedemptionPoints)
                throw new InvalidOperationException($"Minimum redemption is {program.MinRedemptionPoints} points.");

            if (request.Points > membership.AvailablePoints)
                throw new InvalidOperationException("Insufficient points balance.");

            var balanceBefore = membership.AvailablePoints;

            var transaction = new LoyaltyPointTransaction
            {
                Id = Guid.NewGuid(),
                LoyaltyMembershipId = membershipId,
                TransactionType = "Redeem",
                Points = -request.Points,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceBefore - request.Points,
                Description = $"Points redeemed for sales order",
                ReferenceDocumentType = "SalesOrder",
                ReferenceDocumentId = request.SalesOrderId,
                TransactionDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepository.CreateAsync(transaction);

            membership.AvailablePoints -= request.Points;
            membership.TotalRedeemed += request.Points;
            membership.LastActivityAt = DateTime.UtcNow;
            membership.UpdatedAt = DateTime.UtcNow;

            await _membershipRepository.UpdateAsync(membership);

            return MapTransactionToDto(transaction);
        }

        public async Task<LoyaltyPointTransactionDto> AdjustPointsAsync(Guid membershipId, AdjustPointsRequest request)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId)
                ?? throw new InvalidOperationException("Loyalty membership not found.");

            var balanceBefore = membership.AvailablePoints;

            var transaction = new LoyaltyPointTransaction
            {
                Id = Guid.NewGuid(),
                LoyaltyMembershipId = membershipId,
                TransactionType = "Adjustment",
                Points = request.Points,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceBefore + request.Points,
                Description = request.Description,
                ReferenceDocumentType = "ManualAdjustment",
                TransactionDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepository.CreateAsync(transaction);

            membership.AvailablePoints += request.Points;
            if (request.Points > 0) membership.LifetimePoints += request.Points;
            membership.LastActivityAt = DateTime.UtcNow;
            membership.UpdatedAt = DateTime.UtcNow;

            await _membershipRepository.UpdateAsync(membership);

            // Recalculate tier if points were added
            if (request.Points > 0) await RecalculateTierAsync(membershipId);

            return MapTransactionToDto(transaction);
        }

        public async Task<PagedResult<LoyaltyPointTransactionDto>> GetTransactionsAsync(Guid membershipId, PagedRequest request)
        {
            var result = await _transactionRepository.GetByMembershipIdAsync(membershipId, request);

            return new PagedResult<LoyaltyPointTransactionDto>
            {
                Items = result.Items.Select(MapTransactionToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task RecalculateTierAsync(Guid membershipId)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId);
            if (membership == null) return;

            var newTier = await _tierRepository.GetTierForPointsAsync(membership.LoyaltyProgramId, membership.LifetimePoints);
            var newTierId = newTier?.Id;

            if (membership.CurrentTierId != newTierId)
            {
                membership.CurrentTierId = newTierId;
                membership.UpdatedAt = DateTime.UtcNow;
                await _membershipRepository.UpdateAsync(membership);
            }
        }

        // ── Mapping ────────────────────────────────────────────────────────

        private static LoyaltyMembershipDto MapToDto(LoyaltyMembership m) => new()
        {
            Id = m.Id,
            LoyaltyProgramId = m.LoyaltyProgramId,
            ProgramName = m.LoyaltyProgram?.Name,
            CustomerId = m.CustomerId,
            CustomerName = m.Customer?.Name,
            CurrentTierId = m.CurrentTierId,
            CurrentTierName = m.CurrentTier?.Name,
            AvailablePoints = m.AvailablePoints,
            LifetimePoints = m.LifetimePoints,
            TotalRedeemed = m.TotalRedeemed,
            JoinedAt = m.JoinedAt,
            LastActivityAt = m.LastActivityAt,
            IsActive = m.IsActive
        };

        private static LoyaltyPointTransactionDto MapTransactionToDto(LoyaltyPointTransaction t) => new()
        {
            Id = t.Id,
            LoyaltyMembershipId = t.LoyaltyMembershipId,
            TransactionType = t.TransactionType,
            Points = t.Points,
            BalanceBefore = t.BalanceBefore,
            BalanceAfter = t.BalanceAfter,
            Description = t.Description,
            ReferenceDocumentType = t.ReferenceDocumentType,
            ReferenceDocumentNumber = t.ReferenceDocumentNumber,
            ExpiresAt = t.ExpiresAt,
            TransactionDate = t.TransactionDate
        };
    }
}
