using Application.DTOs.Import;
using Application.Interfaces;
using Application.UseCases.Import;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace StockLedger.Unit.Tests.UseCases;

public class CsvImportUseCaseTests
{
    private readonly ICsvImportRepository _importRepo = Substitute.For<ICsvImportRepository>();
    private readonly ISalesChannelRepository _channelRepo = Substitute.For<ISalesChannelRepository>();
    private readonly IProductVariantRepository _variantRepo = Substitute.For<IProductVariantRepository>();
    private readonly IWarehouseStockRepository _stockRepo = Substitute.For<IWarehouseStockRepository>();
    private readonly IStockMovementRepository _movementRepo = Substitute.For<IStockMovementRepository>();
    private readonly IWarehouseRepository _warehouseRepo = Substitute.For<IWarehouseRepository>();
    private readonly ITransactionManager _transactionManager = Substitute.For<ITransactionManager>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICsvParserService _csvParser = Substitute.For<ICsvParserService>();
    private readonly CsvImportUseCase _sut;

    // Shared test data
    private readonly Guid _channelId = Guid.NewGuid();
    private readonly Guid _warehouseId = Guid.NewGuid();
    private readonly SalesChannel _channel;
    private readonly Warehouse _warehouse;

    public CsvImportUseCaseTests()
    {
        // Make the mocked TransactionManager actually invoke the work delegate
        // so the use-case body executes inside the test.
        _transactionManager
            .ExecuteInTransactionAsync(Arg.Any<Func<CancellationToken, Task>>(), Arg.Any<CancellationToken>())
            .Returns(async ci =>
            {
                var work = ci.Arg<Func<CancellationToken, Task>>();
                await work(ci.Arg<CancellationToken>());
            });

        _sut = new CsvImportUseCase(
            _importRepo, _channelRepo, _variantRepo, _stockRepo,
            _movementRepo, _warehouseRepo, _transactionManager, _unitOfWork, _csvParser);

        _channel = new SalesChannel
        {
            Id = _channelId,
            Name = "Tokopedia",
            Slug = "tokopedia",
            PlatformFeePercent = 5m,
            IsActive = true
        };

        _warehouse = new Warehouse
        {
            Id = _warehouseId,
            Name = "Main Warehouse",
            Location = "Jakarta"
        };
    }

    private StartImportRequest CreateImportRequest() => new()
    {
        SalesChannelId = _channelId,
        WarehouseId = _warehouseId,
        ColumnMapping = new CsvColumnMapping
        {
            OrderNumberColumn = "Order No",
            SkuColumn = "SKU",
            QuantityColumn = "Qty",
            UnitPriceColumn = "Price"
        }
    };

    private static Dictionary<string, string> CreateRow(string orderNo, string sku, string qty, string price) =>
        new()
        {
            { "Order No", orderNo },
            { "SKU", sku },
            { "Qty", qty },
            { "Price", price }
        };

    private void SetupChannelAndWarehouse()
    {
        _channelRepo.GetByIdAsync(_channelId).Returns(_channel);
        _warehouseRepo.GetByIdAsync(_warehouseId).Returns(_warehouse);
    }

    // ── Test 1: Happy path ──────────────────────────────────────────────────

    [Fact]
    public async Task ProcessImportAsync_ValidCsv_DeductsStockAndCreatesMovements()
    {
        // Arrange
        SetupChannelAndWarehouse();

        var variant1Id = Guid.NewGuid();
        var variant2Id = Guid.NewGuid();
        var variant1 = new ProductVariant { Id = variant1Id, Sku = "SKU-001", Name = "Widget A" };
        var variant2 = new ProductVariant { Id = variant2Id, Sku = "SKU-002", Name = "Widget B" };

        var stock1 = new WarehouseStock { Id = Guid.NewGuid(), WarehouseId = _warehouseId, ProductVariantId = variant1Id, QuantityOnHand = 50 };
        var stock2 = new WarehouseStock { Id = Guid.NewGuid(), WarehouseId = _warehouseId, ProductVariantId = variant2Id, QuantityOnHand = 30 };

        var rows = new List<Dictionary<string, string>>
        {
            CreateRow("ORD-001", "SKU-001", "5", "10000"),
            CreateRow("ORD-002", "SKU-002", "3", "20000")
        };

        _csvParser.ParseAsync(Arg.Any<Stream>()).Returns(rows);
        _importRepo.CreateAsync(Arg.Any<CsvImportBatch>()).Returns(ci => ci.Arg<CsvImportBatch>());
        _importRepo.OrderNumberExistsForChannel(_channelId, Arg.Any<string>()).Returns(false);
        _variantRepo.GetBySkuAsync("SKU-001").Returns(variant1);
        _variantRepo.GetBySkuAsync("SKU-002").Returns(variant2);
        _stockRepo.GetByWarehouseAndVariantAsync(_warehouseId, variant1Id).Returns(stock1);
        _stockRepo.GetByWarehouseAndVariantAsync(_warehouseId, variant2Id).Returns(stock2);
        _movementRepo.CreateAsync(Arg.Any<StockMovement>()).Returns(ci => ci.Arg<StockMovement>());

        // Act
        var result = await _sut.ProcessImportAsync(Stream.Null, "test.csv", CreateImportRequest());

        // Assert
        result.TotalRows.Should().Be(2);
        result.SuccessRows.Should().Be(2);
        result.FailedRows.Should().Be(0);
        result.SkippedRows.Should().Be(0);
        result.Status.Should().Be(ImportBatchStatus.Completed.ToString());

        stock1.QuantityOnHand.Should().Be(45); // 50 - 5
        stock2.QuantityOnHand.Should().Be(27); // 30 - 3

        await _movementRepo.Received(2).CreateAsync(Arg.Any<StockMovement>());
        await _transactionManager.Received(1).ExecuteInTransactionAsync(
            Arg.Any<Func<CancellationToken, Task>>(), Arg.Any<CancellationToken>());
    }

    // ── Test 2: Unmatched SKU ───────────────────────────────────────────────

    [Fact]
    public async Task ProcessImportAsync_UnmatchedSku_MarksRowAsUnmatched()
    {
        // Arrange
        SetupChannelAndWarehouse();

        var rows = new List<Dictionary<string, string>>
        {
            CreateRow("ORD-001", "UNKNOWN-SKU", "2", "15000")
        };

        _csvParser.ParseAsync(Arg.Any<Stream>()).Returns(rows);
        _importRepo.CreateAsync(Arg.Any<CsvImportBatch>()).Returns(ci => ci.Arg<CsvImportBatch>());
        _importRepo.OrderNumberExistsForChannel(_channelId, Arg.Any<string>()).Returns(false);
        _variantRepo.GetBySkuAsync("UNKNOWN-SKU").ReturnsNull();

        // Act
        var result = await _sut.ProcessImportAsync(Stream.Null, "test.csv", CreateImportRequest());

        // Assert
        result.TotalRows.Should().Be(1);
        result.FailedRows.Should().Be(1);
        result.SuccessRows.Should().Be(0);
        result.FailedRowDetails.Should().HaveCount(1);
        result.FailedRowDetails[0].ErrorMessage.Should().Contain("No product variant found");
        result.FailedRowDetails[0].Status.Should().Be(ImportRowStatus.Unmatched.ToString());
    }

    // ── Test 3: Insufficient stock ──────────────────────────────────────────

    [Fact]
    public async Task ProcessImportAsync_InsufficientStock_MarksRowAsError()
    {
        // Arrange
        SetupChannelAndWarehouse();

        var variantId = Guid.NewGuid();
        var variant = new ProductVariant { Id = variantId, Sku = "SKU-001", Name = "Widget A" };
        var stock = new WarehouseStock { Id = Guid.NewGuid(), WarehouseId = _warehouseId, ProductVariantId = variantId, QuantityOnHand = 2 };

        var rows = new List<Dictionary<string, string>>
        {
            CreateRow("ORD-001", "SKU-001", "10", "15000") // Needs 10 but only 2 available
        };

        _csvParser.ParseAsync(Arg.Any<Stream>()).Returns(rows);
        _importRepo.CreateAsync(Arg.Any<CsvImportBatch>()).Returns(ci => ci.Arg<CsvImportBatch>());
        _importRepo.OrderNumberExistsForChannel(_channelId, Arg.Any<string>()).Returns(false);
        _variantRepo.GetBySkuAsync("SKU-001").Returns(variant);
        _stockRepo.GetByWarehouseAndVariantAsync(_warehouseId, variantId).Returns(stock);

        // Act
        var result = await _sut.ProcessImportAsync(Stream.Null, "test.csv", CreateImportRequest());

        // Assert
        result.TotalRows.Should().Be(1);
        result.FailedRows.Should().Be(1);
        result.SuccessRows.Should().Be(0);
        result.FailedRowDetails.Should().HaveCount(1);
        result.FailedRowDetails[0].Status.Should().Be(ImportRowStatus.Error.ToString());
        result.FailedRowDetails[0].ErrorMessage.Should().Contain("Insufficient stock");
        stock.QuantityOnHand.Should().Be(2); // Stock should remain unchanged
    }

    // ── Test 4: Empty CSV ───────────────────────────────────────────────────

    [Fact]
    public async Task ProcessImportAsync_EmptyCsv_ThrowsInvalidOperation()
    {
        // Arrange
        SetupChannelAndWarehouse();
        _csvParser.ParseAsync(Arg.Any<Stream>()).Returns(new List<Dictionary<string, string>>());

        // Act
        var act = () => _sut.ProcessImportAsync(Stream.Null, "empty.csv", CreateImportRequest());

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*empty*");
    }

    // ── Test 5: Duplicate order ─────────────────────────────────────────────

    [Fact]
    public async Task ProcessImportAsync_DuplicateOrder_SkipsRow()
    {
        // Arrange
        SetupChannelAndWarehouse();

        var rows = new List<Dictionary<string, string>>
        {
            CreateRow("ORD-001", "SKU-001", "2", "15000")
        };

        _csvParser.ParseAsync(Arg.Any<Stream>()).Returns(rows);
        _importRepo.CreateAsync(Arg.Any<CsvImportBatch>()).Returns(ci => ci.Arg<CsvImportBatch>());
        _importRepo.OrderNumberExistsForChannel(_channelId, "ORD-001-SKU-001").Returns(true);

        // Act
        var result = await _sut.ProcessImportAsync(Stream.Null, "test.csv", CreateImportRequest());

        // Assert
        result.TotalRows.Should().Be(1);
        result.SkippedRows.Should().Be(1);
        result.SuccessRows.Should().Be(0);
        result.FailedRows.Should().Be(0);
        await _movementRepo.DidNotReceive().CreateAsync(Arg.Any<StockMovement>());
    }

    // ── Test 6: Duplicate channel slug ──────────────────────────────────────

    [Fact]
    public async Task CreateChannelAsync_DuplicateSlug_ThrowsInvalidOperation()
    {
        // Arrange
        var existing = new SalesChannel { Id = Guid.NewGuid(), Name = "Tokopedia", Slug = "tokopedia" };
        _channelRepo.GetBySlugAsync("tokopedia").Returns(existing);

        var request = new CreateSalesChannelRequest
        {
            Name = "Tokopedia",
            Description = "Duplicate channel",
            PlatformFeePercent = 5m
        };

        // Act
        var act = () => _sut.CreateChannelAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    // ── Test 7: Create new channel ──────────────────────────────────────────

    [Fact]
    public async Task CreateChannelAsync_NewChannel_Succeeds()
    {
        // Arrange
        _channelRepo.GetBySlugAsync("shopee").ReturnsNull();
        _channelRepo.CreateAsync(Arg.Any<SalesChannel>()).Returns(ci => ci.Arg<SalesChannel>());

        var request = new CreateSalesChannelRequest
        {
            Name = "Shopee",
            Description = "Shopee marketplace",
            PlatformFeePercent = 3.5m
        };

        // Act
        var result = await _sut.CreateChannelAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Shopee");
        result.Slug.Should().Be("shopee");
        result.PlatformFeePercent.Should().Be(3.5m);
        result.IsActive.Should().BeTrue();
        result.Id.Should().NotBeEmpty();

        await _channelRepo.Received(1).CreateAsync(Arg.Any<SalesChannel>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
