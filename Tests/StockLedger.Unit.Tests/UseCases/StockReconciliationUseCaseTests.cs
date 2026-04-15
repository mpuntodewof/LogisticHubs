using Application.DTOs.Inventory;
using Application.Interfaces;
using Application.UseCases.Inventory;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace StockLedger.Unit.Tests.UseCases;

public class StockReconciliationUseCaseTests
{
    private readonly IWarehouseStockRepository _stockRepo = Substitute.For<IWarehouseStockRepository>();
    private readonly IStockMovementRepository _movementRepo = Substitute.For<IStockMovementRepository>();
    private readonly IProductVariantRepository _variantRepo = Substitute.For<IProductVariantRepository>();
    private readonly ITransactionManager _transactionManager = Substitute.For<ITransactionManager>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly StockReconciliationUseCase _sut;

    private readonly Guid _warehouseId = Guid.NewGuid();

    public StockReconciliationUseCaseTests()
    {
        _sut = new StockReconciliationUseCase(
            _stockRepo, _movementRepo, _variantRepo, _transactionManager, _unitOfWork);

        _movementRepo.CreateAsync(Arg.Any<StockMovement>()).Returns(ci => ci.Arg<StockMovement>());
        _stockRepo.CreateAsync(Arg.Any<WarehouseStock>()).Returns(ci => ci.Arg<WarehouseStock>());
    }

    private ProductVariant CreateVariant(Guid id, string sku = "SKU-001", string name = "Variant A")
    {
        return new ProductVariant
        {
            Id = id,
            Sku = sku,
            Name = name,
            Product = new Product { Id = Guid.NewGuid(), Name = "Test Product" }
        };
    }

    private WarehouseStock CreateStock(Guid variantId, int qty)
    {
        return new WarehouseStock
        {
            Id = Guid.NewGuid(),
            WarehouseId = _warehouseId,
            ProductVariantId = variantId,
            QuantityOnHand = qty,
            TenantId = Guid.NewGuid()
        };
    }

    [Fact]
    public async Task ReconcileAsync_MatchingCounts_NoAdjustments()
    {
        var variantId = Guid.NewGuid();
        var stock = CreateStock(variantId, 40);
        var variant = CreateVariant(variantId);

        _stockRepo.GetByWarehouseAndVariantAsync(_warehouseId, variantId).Returns(stock);
        _variantRepo.GetByIdAsync(variantId).Returns(variant);

        var request = new StockReconciliationRequest
        {
            WarehouseId = _warehouseId,
            Counts = new List<StockCountLine>
            {
                new() { ProductVariantId = variantId, PhysicalCount = 40 }
            }
        };

        var result = await _sut.ReconcileAsync(request);

        result.MatchedItems.Should().Be(1);
        result.AdjustedItems.Should().Be(0);
        result.TotalItems.Should().Be(1);
        result.Variances.Should().HaveCount(1);
        result.Variances[0].Variance.Should().Be(0);
        await _movementRepo.DidNotReceive().CreateAsync(Arg.Any<StockMovement>());
        await _transactionManager.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReconcileAsync_OverCount_CreatesAdjustmentMovement()
    {
        var variantId = Guid.NewGuid();
        var stock = CreateStock(variantId, 40);
        var variant = CreateVariant(variantId);

        _stockRepo.GetByWarehouseAndVariantAsync(_warehouseId, variantId).Returns(stock);
        _variantRepo.GetByIdAsync(variantId).Returns(variant);

        var request = new StockReconciliationRequest
        {
            WarehouseId = _warehouseId,
            Counts = new List<StockCountLine>
            {
                new() { ProductVariantId = variantId, PhysicalCount = 50 }
            }
        };

        var result = await _sut.ReconcileAsync(request);

        result.AdjustedItems.Should().Be(1);
        result.Variances[0].Variance.Should().Be(10);
        stock.QuantityOnHand.Should().Be(50);

        await _movementRepo.Received(1).CreateAsync(Arg.Is<StockMovement>(m =>
            m.QuantityBefore == 40 &&
            m.QuantityAfter == 50 &&
            m.Quantity == 10));
        await _stockRepo.Received(1).UpdateAsync(stock);
    }

    [Fact]
    public async Task ReconcileAsync_UnderCount_CreatesAdjustmentMovement()
    {
        var variantId = Guid.NewGuid();
        var stock = CreateStock(variantId, 50);
        var variant = CreateVariant(variantId);

        _stockRepo.GetByWarehouseAndVariantAsync(_warehouseId, variantId).Returns(stock);
        _variantRepo.GetByIdAsync(variantId).Returns(variant);

        var request = new StockReconciliationRequest
        {
            WarehouseId = _warehouseId,
            Counts = new List<StockCountLine>
            {
                new() { ProductVariantId = variantId, PhysicalCount = 30 }
            }
        };

        var result = await _sut.ReconcileAsync(request);

        result.AdjustedItems.Should().Be(1);
        result.Variances[0].Variance.Should().Be(-20);
        stock.QuantityOnHand.Should().Be(30);

        await _movementRepo.Received(1).CreateAsync(Arg.Is<StockMovement>(m =>
            m.QuantityBefore == 50 &&
            m.QuantityAfter == 30 &&
            m.Quantity == 20));
        await _stockRepo.Received(1).UpdateAsync(stock);
    }

    [Fact]
    public async Task ReconcileAsync_NewStock_CreatesWarehouseStock()
    {
        var variantId = Guid.NewGuid();
        var variant = CreateVariant(variantId);

        _stockRepo.GetByWarehouseAndVariantAsync(_warehouseId, variantId).ReturnsNull();
        _variantRepo.GetByIdAsync(variantId).Returns(variant);

        var request = new StockReconciliationRequest
        {
            WarehouseId = _warehouseId,
            Counts = new List<StockCountLine>
            {
                new() { ProductVariantId = variantId, PhysicalCount = 25 }
            }
        };

        var result = await _sut.ReconcileAsync(request);

        result.AdjustedItems.Should().Be(1);
        result.Variances[0].SystemCount.Should().Be(0);
        result.Variances[0].PhysicalCount.Should().Be(25);

        await _stockRepo.Received(1).CreateAsync(Arg.Is<WarehouseStock>(s =>
            s.WarehouseId == _warehouseId &&
            s.ProductVariantId == variantId &&
            s.QuantityOnHand == 25));
        await _movementRepo.Received(1).CreateAsync(Arg.Is<StockMovement>(m =>
            m.QuantityBefore == 0 &&
            m.QuantityAfter == 25));
    }

    [Fact]
    public async Task ReconcileAsync_EmptyRequest_ThrowsInvalidOperation()
    {
        var request = new StockReconciliationRequest
        {
            WarehouseId = _warehouseId,
            Counts = new List<StockCountLine>()
        };

        var act = () => _sut.ReconcileAsync(request);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*At least one stock count*");
    }

    [Fact]
    public async Task ReconcileAsync_MultipleItems_ProcessesAll()
    {
        var matchId = Guid.NewGuid();
        var overId = Guid.NewGuid();
        var underId = Guid.NewGuid();

        var matchStock = CreateStock(matchId, 100);
        var overStock = CreateStock(overId, 40);
        var underStock = CreateStock(underId, 60);

        _stockRepo.GetByWarehouseAndVariantAsync(_warehouseId, matchId).Returns(matchStock);
        _stockRepo.GetByWarehouseAndVariantAsync(_warehouseId, overId).Returns(overStock);
        _stockRepo.GetByWarehouseAndVariantAsync(_warehouseId, underId).Returns(underStock);

        _variantRepo.GetByIdAsync(matchId).Returns(CreateVariant(matchId, "SKU-MATCH", "Match"));
        _variantRepo.GetByIdAsync(overId).Returns(CreateVariant(overId, "SKU-OVER", "Over"));
        _variantRepo.GetByIdAsync(underId).Returns(CreateVariant(underId, "SKU-UNDER", "Under"));

        var request = new StockReconciliationRequest
        {
            WarehouseId = _warehouseId,
            Counts = new List<StockCountLine>
            {
                new() { ProductVariantId = matchId, PhysicalCount = 100 },
                new() { ProductVariantId = overId, PhysicalCount = 50 },
                new() { ProductVariantId = underId, PhysicalCount = 45 }
            }
        };

        var result = await _sut.ReconcileAsync(request);

        result.TotalItems.Should().Be(3);
        result.MatchedItems.Should().Be(1);
        result.AdjustedItems.Should().Be(2);
        result.Variances.Should().HaveCount(3);
        result.Variances[0].Variance.Should().Be(0);
        result.Variances[1].Variance.Should().Be(10);
        result.Variances[2].Variance.Should().Be(-15);

        await _movementRepo.Received(2).CreateAsync(Arg.Any<StockMovement>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _transactionManager.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
