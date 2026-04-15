using Application.DTOs.Inventory;
using Application.Interfaces;
using Application.UseCases.Inventory;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace StockLedger.Unit.Tests.UseCases;

public class StockMovementUseCaseTests
{
    private readonly IStockMovementRepository _movementRepo = Substitute.For<IStockMovementRepository>();
    private readonly IWarehouseStockRepository _stockRepo = Substitute.For<IWarehouseStockRepository>();
    private readonly ITransactionManager _transactionManager = Substitute.For<ITransactionManager>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly StockMovementUseCase _sut;

    public StockMovementUseCaseTests()
    {
        _sut = new StockMovementUseCase(_movementRepo, _stockRepo, _transactionManager, _unitOfWork);
    }

    [Fact]
    public async Task CreateMovementAsync_StockIn_IncreasesQuantity()
    {
        var warehouseId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var stock = new WarehouseStock
        {
            Id = Guid.NewGuid(),
            WarehouseId = warehouseId,
            ProductVariantId = variantId,
            QuantityOnHand = 50,
            TenantId = Guid.NewGuid()
        };

        _stockRepo.GetByWarehouseAndVariantAsync(warehouseId, variantId).Returns(stock);
        _movementRepo.CreateAsync(Arg.Any<StockMovement>()).Returns(ci => ci.Arg<StockMovement>());

        var request = new CreateStockMovementRequest
        {
            WarehouseId = warehouseId,
            ProductVariantId = variantId,
            MovementType = StockMovementType.In,
            Reason = StockMovementReason.Purchase,
            Quantity = 10
        };

        var result = await _sut.CreateMovementAsync(request);

        stock.QuantityOnHand.Should().Be(60);
        result.QuantityBefore.Should().Be(50);
        result.QuantityAfter.Should().Be(60);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _transactionManager.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateMovementAsync_StockOut_DecreasesQuantity()
    {
        var warehouseId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var stock = new WarehouseStock
        {
            Id = Guid.NewGuid(),
            WarehouseId = warehouseId,
            ProductVariantId = variantId,
            QuantityOnHand = 50,
            TenantId = Guid.NewGuid()
        };

        _stockRepo.GetByWarehouseAndVariantAsync(warehouseId, variantId).Returns(stock);
        _movementRepo.CreateAsync(Arg.Any<StockMovement>()).Returns(ci => ci.Arg<StockMovement>());

        var request = new CreateStockMovementRequest
        {
            WarehouseId = warehouseId,
            ProductVariantId = variantId,
            MovementType = StockMovementType.Out,
            Reason = StockMovementReason.Sale,
            Quantity = 20
        };

        var result = await _sut.CreateMovementAsync(request);

        stock.QuantityOnHand.Should().Be(30);
        result.QuantityBefore.Should().Be(50);
        result.QuantityAfter.Should().Be(30);
    }

    [Fact]
    public async Task CreateMovementAsync_InsufficientStock_ThrowsInvalidOperation()
    {
        var warehouseId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var stock = new WarehouseStock
        {
            Id = Guid.NewGuid(),
            WarehouseId = warehouseId,
            ProductVariantId = variantId,
            QuantityOnHand = 5,
            TenantId = Guid.NewGuid()
        };

        _stockRepo.GetByWarehouseAndVariantAsync(warehouseId, variantId).Returns(stock);

        var request = new CreateStockMovementRequest
        {
            WarehouseId = warehouseId,
            ProductVariantId = variantId,
            MovementType = StockMovementType.Out,
            Reason = StockMovementReason.Sale,
            Quantity = 20
        };

        var act = () => _sut.CreateMovementAsync(request);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Insufficient stock*");
        await _transactionManager.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateMovementAsync_NewStock_CreatesWarehouseStock()
    {
        var warehouseId = Guid.NewGuid();
        var variantId = Guid.NewGuid();

        _stockRepo.GetByWarehouseAndVariantAsync(warehouseId, variantId).ReturnsNull();
        _stockRepo.CreateAsync(Arg.Any<WarehouseStock>()).Returns(ci => ci.Arg<WarehouseStock>());
        _movementRepo.CreateAsync(Arg.Any<StockMovement>()).Returns(ci => ci.Arg<StockMovement>());

        var request = new CreateStockMovementRequest
        {
            WarehouseId = warehouseId,
            ProductVariantId = variantId,
            MovementType = StockMovementType.In,
            Reason = StockMovementReason.InitialStock,
            Quantity = 100
        };

        var result = await _sut.CreateMovementAsync(request);

        result.QuantityBefore.Should().Be(0);
        result.QuantityAfter.Should().Be(100);
        await _stockRepo.Received(1).CreateAsync(Arg.Any<WarehouseStock>());
    }

    [Fact]
    public async Task CreateTransferAsync_SameWarehouse_ThrowsInvalidOperation()
    {
        var warehouseId = Guid.NewGuid();

        var request = new CreateStockTransferRequest
        {
            SourceWarehouseId = warehouseId,
            DestinationWarehouseId = warehouseId,
            ProductVariantId = Guid.NewGuid(),
            Quantity = 10
        };

        var act = () => _sut.CreateTransferAsync(request);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Source and destination*must be different*");
    }
}
