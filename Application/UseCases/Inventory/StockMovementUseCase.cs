using Application.DTOs.Common;
using Application.DTOs.Inventory;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using ConcurrencyConflictException = Application.Interfaces.ConcurrencyConflictException;

namespace Application.UseCases.Inventory
{
    public class StockMovementUseCase
    {
        private const int MaxConcurrencyRetries = 3;

        private readonly IStockMovementRepository _movementRepository;
        private readonly IWarehouseStockRepository _stockRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly IUnitOfWork _unitOfWork;

        public StockMovementUseCase(
            IStockMovementRepository movementRepository,
            IWarehouseStockRepository stockRepository,
            ITransactionManager transactionManager,
            IUnitOfWork unitOfWork)
        {
            _movementRepository = movementRepository;
            _stockRepository = stockRepository;
            _transactionManager = transactionManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<StockMovementDto>> GetPagedAsync(
            PagedRequest request, Guid? warehouseId = null, Guid? productVariantId = null, string? movementType = null)
        {
            var result = await _movementRepository.GetPagedAsync(request, warehouseId, productVariantId, movementType);

            return new PagedResult<StockMovementDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<StockMovementDto?> GetByIdAsync(Guid id)
        {
            var movement = await _movementRepository.GetByIdAsync(id);
            return movement == null ? null : MapToDto(movement);
        }

        public async Task<StockMovementDto> CreateMovementAsync(CreateStockMovementRequest request)
        {
            for (int attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                try
                {
                    StockMovementDto? dto = null;
                    await _transactionManager.ExecuteInTransactionAsync(async _ =>
                    {
                        // Get or create warehouse stock
                        var stock = await _stockRepository.GetByWarehouseAndVariantAsync(request.WarehouseId, request.ProductVariantId);
                        bool isNewStock = stock == null;

                        if (isNewStock)
                        {
                            stock = new WarehouseStock
                            {
                                Id = Guid.NewGuid(),
                                WarehouseId = request.WarehouseId,
                                ProductVariantId = request.ProductVariantId,
                                QuantityOnHand = 0,
                                QuantityReserved = 0,
                                CreatedAt = DateTime.UtcNow
                            };
                        }

                        var quantityBefore = stock!.QuantityOnHand;

                        switch (request.MovementType)
                        {
                            case StockMovementType.In:
                                stock.QuantityOnHand += request.Quantity;
                                break;

                            case StockMovementType.Out:
                                if (stock.QuantityOnHand < request.Quantity)
                                    throw new InvalidOperationException("Insufficient stock for outbound movement.");
                                stock.QuantityOnHand -= request.Quantity;
                                break;

                            case StockMovementType.Adjustment:
                                stock.QuantityOnHand = quantityBefore + request.Quantity;
                                break;

                            default:
                                throw new InvalidOperationException($"Unsupported movement type: {request.MovementType}");
                        }

                        var quantityAfter = stock.QuantityOnHand;

                        var movement = new StockMovement
                        {
                            Id = Guid.NewGuid(),
                            WarehouseId = request.WarehouseId,
                            ProductVariantId = request.ProductVariantId,
                            MovementType = request.MovementType.ToString(),
                            Reason = request.Reason.ToString(),
                            Quantity = request.Quantity,
                            QuantityBefore = quantityBefore,
                            QuantityAfter = quantityAfter,
                            ReferenceDocumentType = request.ReferenceDocumentType,
                            ReferenceDocumentId = request.ReferenceDocumentId,
                            ReferenceDocumentNumber = request.ReferenceDocumentNumber,
                            Notes = request.Notes,
                            CreatedAt = DateTime.UtcNow
                        };

                        if (isNewStock)
                            await _stockRepository.CreateAsync(stock);
                        else
                            await _stockRepository.UpdateAsync(stock);

                        var created = await _movementRepository.CreateAsync(movement);

                        await _unitOfWork.SaveChangesAsync();
                        dto = MapToDto(created);
                    });

                    return dto!;
                }
                catch (ConcurrencyConflictException) when (attempt < MaxConcurrencyRetries)
                {
                    // Retry with fresh data on next iteration
                    continue;
                }
            }

            throw new InvalidOperationException("Stock update failed after multiple retries due to concurrent modifications.");
        }

        public async Task<StockMovementDto> RecordManualSaleAsync(RecordManualSaleRequest request)
        {
            return await CreateMovementAsync(new CreateStockMovementRequest
            {
                WarehouseId = request.WarehouseId,
                ProductVariantId = request.ProductVariantId,
                MovementType = StockMovementType.Out,
                Reason = StockMovementReason.Sale,
                Quantity = request.Quantity,
                ReferenceDocumentType = "Manual-Sale",
                ReferenceDocumentNumber = request.ReceiptNumber,
                Notes = request.Notes
            });
        }

        public async Task<StockMovementDto> CreateTransferAsync(CreateStockTransferRequest request)
        {
            if (request.SourceWarehouseId == request.DestinationWarehouseId)
                throw new InvalidOperationException("Source and destination warehouses must be different.");

            StockMovementDto? transferDto = null;
            await _transactionManager.ExecuteInTransactionAsync(async _ =>
            {
                // Get or create source stock
                var sourceStock = await _stockRepository.GetByWarehouseAndVariantAsync(
                    request.SourceWarehouseId, request.ProductVariantId);
                bool isNewSourceStock = sourceStock == null;

                if (isNewSourceStock)
                {
                    sourceStock = new WarehouseStock
                    {
                        Id = Guid.NewGuid(),
                        WarehouseId = request.SourceWarehouseId,
                        ProductVariantId = request.ProductVariantId,
                        QuantityOnHand = 0,
                        QuantityReserved = 0,
                        CreatedAt = DateTime.UtcNow
                    };
                }

                if (sourceStock!.QuantityOnHand < request.Quantity)
                    throw new InvalidOperationException("Insufficient stock in source warehouse for transfer.");

                // Get or create destination stock
                var destStock = await _stockRepository.GetByWarehouseAndVariantAsync(
                    request.DestinationWarehouseId, request.ProductVariantId);
                bool isNewDestStock = destStock == null;

                if (isNewDestStock)
                {
                    destStock = new WarehouseStock
                    {
                        Id = Guid.NewGuid(),
                        WarehouseId = request.DestinationWarehouseId,
                        ProductVariantId = request.ProductVariantId,
                        QuantityOnHand = 0,
                        QuantityReserved = 0,
                        CreatedAt = DateTime.UtcNow
                    };
                }

                var sourceQtyBefore = sourceStock.QuantityOnHand;
                var destQtyBefore = destStock!.QuantityOnHand;

                sourceStock.QuantityOnHand -= request.Quantity;
                destStock.QuantityOnHand += request.Quantity;

                // Create OUT movement from source
                var outMovement = new StockMovement
                {
                    Id = Guid.NewGuid(),
                    WarehouseId = request.SourceWarehouseId,
                    ProductVariantId = request.ProductVariantId,
                    MovementType = StockMovementType.Transfer.ToString(),
                    Reason = StockMovementReason.TransferOut.ToString(),
                    Quantity = request.Quantity,
                    QuantityBefore = sourceQtyBefore,
                    QuantityAfter = sourceStock.QuantityOnHand,
                    SourceWarehouseId = request.SourceWarehouseId,
                    DestinationWarehouseId = request.DestinationWarehouseId,
                    ReferenceDocumentNumber = request.ReferenceDocumentNumber,
                    Notes = request.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                // Create IN movement to destination
                var inMovement = new StockMovement
                {
                    Id = Guid.NewGuid(),
                    WarehouseId = request.DestinationWarehouseId,
                    ProductVariantId = request.ProductVariantId,
                    MovementType = StockMovementType.Transfer.ToString(),
                    Reason = StockMovementReason.TransferIn.ToString(),
                    Quantity = request.Quantity,
                    QuantityBefore = destQtyBefore,
                    QuantityAfter = destStock.QuantityOnHand,
                    SourceWarehouseId = request.SourceWarehouseId,
                    DestinationWarehouseId = request.DestinationWarehouseId,
                    ReferenceDocumentNumber = request.ReferenceDocumentNumber,
                    Notes = request.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                // Update or create stocks
                if (isNewSourceStock)
                    await _stockRepository.CreateAsync(sourceStock);
                else
                    await _stockRepository.UpdateAsync(sourceStock);

                if (isNewDestStock)
                    await _stockRepository.CreateAsync(destStock);
                else
                    await _stockRepository.UpdateAsync(destStock);

                // Create both movement records
                var createdOut = await _movementRepository.CreateAsync(outMovement);
                await _movementRepository.CreateAsync(inMovement);

                await _unitOfWork.SaveChangesAsync();
                transferDto = MapToDto(createdOut);
            });

            return transferDto!;
        }

        private static StockMovementDto MapToDto(StockMovement m) => new()
        {
            Id = m.Id,
            WarehouseId = m.WarehouseId,
            WarehouseName = m.Warehouse?.Name ?? string.Empty,
            ProductVariantId = m.ProductVariantId,
            Sku = m.ProductVariant?.Sku ?? string.Empty,
            ProductVariantName = m.ProductVariant?.Name ?? string.Empty,
            MovementType = m.MovementType,
            Reason = m.Reason,
            Quantity = m.Quantity,
            QuantityBefore = m.QuantityBefore,
            QuantityAfter = m.QuantityAfter,
            ReferenceDocumentType = m.ReferenceDocumentType,
            ReferenceDocumentId = m.ReferenceDocumentId,
            ReferenceDocumentNumber = m.ReferenceDocumentNumber,
            SourceWarehouseId = m.SourceWarehouseId,
            DestinationWarehouseId = m.DestinationWarehouseId,
            Notes = m.Notes,
            CreatedAt = m.CreatedAt,
            CreatedBy = m.CreatedBy
        };
    }
}
