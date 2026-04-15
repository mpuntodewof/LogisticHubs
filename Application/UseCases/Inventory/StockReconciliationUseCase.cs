using Application.DTOs.Inventory;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Inventory
{
    public class StockReconciliationUseCase
    {
        private readonly IWarehouseStockRepository _stockRepository;
        private readonly IStockMovementRepository _movementRepository;
        private readonly IProductVariantRepository _variantRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly IUnitOfWork _unitOfWork;

        public StockReconciliationUseCase(
            IWarehouseStockRepository stockRepository,
            IStockMovementRepository movementRepository,
            IProductVariantRepository variantRepository,
            ITransactionManager transactionManager,
            IUnitOfWork unitOfWork)
        {
            _stockRepository = stockRepository;
            _movementRepository = movementRepository;
            _variantRepository = variantRepository;
            _transactionManager = transactionManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<StockReconciliationResult> ReconcileAsync(StockReconciliationRequest request)
        {
            if (request.Counts.Count == 0)
                throw new InvalidOperationException("At least one stock count is required.");

            var result = new StockReconciliationResult { TotalItems = request.Counts.Count };

            await _transactionManager.BeginTransactionAsync();
            try
            {
                foreach (var count in request.Counts)
                {
                    var stock = await _stockRepository.GetByWarehouseAndVariantAsync(
                        request.WarehouseId, count.ProductVariantId);

                    var variant = await _variantRepository.GetByIdAsync(count.ProductVariantId);
                    var systemCount = stock?.QuantityOnHand ?? 0;
                    var variance = count.PhysicalCount - systemCount;

                    var varianceLine = new StockVarianceLine
                    {
                        ProductVariantId = count.ProductVariantId,
                        Sku = variant?.Sku ?? "Unknown",
                        ProductName = variant?.Product?.Name != null
                            ? $"{variant.Product.Name} - {variant.Name}"
                            : variant?.Name ?? "Unknown",
                        SystemCount = systemCount,
                        PhysicalCount = count.PhysicalCount,
                        Variance = variance
                    };
                    result.Variances.Add(varianceLine);

                    if (variance == 0)
                    {
                        result.MatchedItems++;
                        continue;
                    }

                    // Create or update stock
                    if (stock == null)
                    {
                        stock = new WarehouseStock
                        {
                            Id = Guid.NewGuid(),
                            WarehouseId = request.WarehouseId,
                            ProductVariantId = count.ProductVariantId,
                            QuantityOnHand = count.PhysicalCount,
                            QuantityReserved = 0,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _stockRepository.CreateAsync(stock);
                    }
                    else
                    {
                        stock.QuantityOnHand = count.PhysicalCount;
                        await _stockRepository.UpdateAsync(stock);
                    }

                    // Create adjustment movement
                    var movement = new StockMovement
                    {
                        Id = Guid.NewGuid(),
                        WarehouseId = request.WarehouseId,
                        ProductVariantId = count.ProductVariantId,
                        MovementType = StockMovementType.Adjustment.ToString(),
                        Reason = StockMovementReason.StockTake.ToString(),
                        Quantity = Math.Abs(variance),
                        QuantityBefore = systemCount,
                        QuantityAfter = count.PhysicalCount,
                        ReferenceDocumentType = "StockReconciliation",
                        Notes = request.Notes ?? $"Physical count adjustment: {systemCount} -> {count.PhysicalCount}",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _movementRepository.CreateAsync(movement);

                    result.AdjustedItems++;
                }

                await _unitOfWork.SaveChangesAsync();
                await _transactionManager.CommitAsync();
                return result;
            }
            catch
            {
                await _transactionManager.RollbackAsync();
                throw;
            }
        }
    }
}
