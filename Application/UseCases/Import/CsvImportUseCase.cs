using Application.DTOs.Common;
using Application.DTOs.Import;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Import
{
    public class CsvImportUseCase
    {
        private readonly ICsvImportRepository _importRepository;
        private readonly ISalesChannelRepository _channelRepository;
        private readonly IProductVariantRepository _variantRepository;
        private readonly IWarehouseStockRepository _stockRepository;
        private readonly IStockMovementRepository _movementRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICsvParserService _csvParser;

        public CsvImportUseCase(
            ICsvImportRepository importRepository,
            ISalesChannelRepository channelRepository,
            IProductVariantRepository variantRepository,
            IWarehouseStockRepository stockRepository,
            IStockMovementRepository movementRepository,
            IWarehouseRepository warehouseRepository,
            ITransactionManager transactionManager,
            IUnitOfWork unitOfWork,
            ICsvParserService csvParser)
        {
            _importRepository = importRepository;
            _channelRepository = channelRepository;
            _variantRepository = variantRepository;
            _stockRepository = stockRepository;
            _movementRepository = movementRepository;
            _warehouseRepository = warehouseRepository;
            _transactionManager = transactionManager;
            _unitOfWork = unitOfWork;
            _csvParser = csvParser;
        }

        // ── Import Batches ───────────────────────────────────────────────────────

        public async Task<PagedResult<CsvImportBatchDto>> GetBatchesAsync(PagedRequest request)
        {
            var result = await _importRepository.GetPagedAsync(request);
            return new PagedResult<CsvImportBatchDto>
            {
                Items = result.Items.Select(MapBatchToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<CsvImportBatchDetailDto?> GetBatchDetailAsync(Guid id)
        {
            var batch = await _importRepository.GetDetailByIdAsync(id);
            return batch == null ? null : MapBatchToDetailDto(batch);
        }

        // ── CSV Headers Preview ──────────────────────────────────────────────────

        public List<string> GetCsvHeaders(Stream csvStream)
        {
            return _csvParser.GetHeaders(csvStream);
        }

        // ── Process Import ───────────────────────────────────────────────────────

        public async Task<ImportSummaryDto> ProcessImportAsync(
            Stream csvStream, string fileName, StartImportRequest request)
        {
            var channel = await _channelRepository.GetByIdAsync(request.SalesChannelId)
                ?? throw new KeyNotFoundException("Sales channel not found.");

            var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId)
                ?? throw new KeyNotFoundException("Warehouse not found.");

            var rows = await _csvParser.ParseAsync(csvStream);
            if (rows.Count == 0)
                throw new InvalidOperationException("CSV file is empty or has no data rows.");

            var batch = new CsvImportBatch
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                SalesChannelId = channel.Id,
                WarehouseId = warehouse.Id,
                Status = ImportBatchStatus.Processing.ToString(),
                TotalRows = rows.Count,
                CreatedAt = DateTime.UtcNow
            };

            var mapping = request.ColumnMapping;
            int successCount = 0, failedCount = 0, skippedCount = 0;
            var failedDetails = new List<CsvImportRowDto>();

            await _transactionManager.BeginTransactionAsync();
            try
            {
                await _importRepository.CreateAsync(batch);

                for (int i = 0; i < rows.Count; i++)
                {
                    var row = rows[i];
                    var importRow = new CsvImportRow
                    {
                        Id = Guid.NewGuid(),
                        CsvImportBatchId = batch.Id,
                        RowNumber = i + 1,
                        RawRowJson = System.Text.Json.JsonSerializer.Serialize(row),
                        CreatedAt = DateTime.UtcNow
                    };

                    try
                    {
                        // Extract fields using column mapping
                        var orderNumber = GetMappedValue(row, mapping.OrderNumberColumn);
                        var sku = GetMappedValue(row, mapping.SkuColumn);
                        var quantityStr = GetMappedValue(row, mapping.QuantityColumn);
                        var unitPriceStr = GetMappedValue(row, mapping.UnitPriceColumn);

                        importRow.OrderNumber = orderNumber;
                        importRow.Sku = sku;
                        importRow.ProductName = mapping.ProductNameColumn != null
                            ? GetMappedValue(row, mapping.ProductNameColumn) : null;

                        if (!int.TryParse(quantityStr?.Replace(",", "").Replace(".", ""), out var quantity) || quantity <= 0)
                        {
                            importRow.Status = ImportRowStatus.Error.ToString();
                            importRow.ErrorMessage = $"Invalid quantity: '{quantityStr}'";
                            failedCount++;
                            batch.Rows.Add(importRow);
                            failedDetails.Add(MapRowToDto(importRow));
                            continue;
                        }

                        importRow.Quantity = quantity;
                        importRow.UnitPrice = ParseDecimal(unitPriceStr);

                        if (mapping.TotalPriceColumn != null)
                            importRow.TotalPrice = ParseDecimal(GetMappedValue(row, mapping.TotalPriceColumn));
                        else
                            importRow.TotalPrice = importRow.UnitPrice * quantity;

                        if (mapping.PlatformFeeColumn != null)
                            importRow.PlatformFee = ParseDecimal(GetMappedValue(row, mapping.PlatformFeeColumn));
                        else
                            importRow.PlatformFee = importRow.TotalPrice * (channel.PlatformFeePercent / 100m);

                        if (mapping.OrderDateColumn != null)
                        {
                            var dateStr = GetMappedValue(row, mapping.OrderDateColumn);
                            if (DateTime.TryParse(dateStr, out var orderDate))
                                importRow.OrderDate = orderDate;
                        }

                        // Duplicate detection
                        if (!string.IsNullOrWhiteSpace(orderNumber) && !string.IsNullOrWhiteSpace(sku))
                        {
                            var isDuplicate = await _importRepository.OrderNumberExistsForChannel(
                                channel.Id, $"{orderNumber}-{sku}");
                            if (isDuplicate)
                            {
                                importRow.Status = ImportRowStatus.Duplicate.ToString();
                                importRow.ErrorMessage = "Order already imported";
                                skippedCount++;
                                batch.Rows.Add(importRow);
                                continue;
                            }
                        }

                        // SKU matching
                        if (string.IsNullOrWhiteSpace(sku))
                        {
                            importRow.Status = ImportRowStatus.Error.ToString();
                            importRow.ErrorMessage = "SKU is empty";
                            failedCount++;
                            batch.Rows.Add(importRow);
                            failedDetails.Add(MapRowToDto(importRow));
                            continue;
                        }

                        var variant = await _variantRepository.GetBySkuAsync(sku);
                        if (variant == null)
                        {
                            importRow.Status = ImportRowStatus.Unmatched.ToString();
                            importRow.ErrorMessage = $"No product variant found for SKU: {sku}";
                            failedCount++;
                            batch.Rows.Add(importRow);
                            failedDetails.Add(MapRowToDto(importRow));
                            continue;
                        }

                        importRow.MatchedProductVariantId = variant.Id;

                        // Stock deduction
                        var stock = await _stockRepository.GetByWarehouseAndVariantAsync(warehouse.Id, variant.Id);
                        if (stock == null || stock.QuantityOnHand < quantity)
                        {
                            importRow.Status = ImportRowStatus.Error.ToString();
                            importRow.ErrorMessage = $"Insufficient stock. Available: {stock?.QuantityOnHand ?? 0}, Required: {quantity}";
                            failedCount++;
                            batch.Rows.Add(importRow);
                            failedDetails.Add(MapRowToDto(importRow));
                            continue;
                        }

                        var quantityBefore = stock.QuantityOnHand;
                        stock.QuantityOnHand -= quantity;

                        var movement = new StockMovement
                        {
                            Id = Guid.NewGuid(),
                            WarehouseId = warehouse.Id,
                            ProductVariantId = variant.Id,
                            MovementType = StockMovementType.Out.ToString(),
                            Reason = StockMovementReason.Sale.ToString(),
                            Quantity = quantity,
                            QuantityBefore = quantityBefore,
                            QuantityAfter = stock.QuantityOnHand,
                            ReferenceDocumentType = $"CSV-{channel.Slug}",
                            ReferenceDocumentId = batch.Id,
                            ReferenceDocumentNumber = orderNumber,
                            Notes = $"Imported from {channel.Name} CSV",
                            CreatedAt = DateTime.UtcNow
                        };

                        await _movementRepository.CreateAsync(movement);
                        importRow.StockMovementId = movement.Id;
                        importRow.Status = ImportRowStatus.Matched.ToString();
                        // Use composite key for duplicate detection
                        importRow.OrderNumber = $"{orderNumber}-{sku}";
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        importRow.Status = ImportRowStatus.Error.ToString();
                        importRow.ErrorMessage = ex.Message;
                        failedCount++;
                        failedDetails.Add(MapRowToDto(importRow));
                    }

                    batch.Rows.Add(importRow);
                }

                batch.SuccessRows = successCount;
                batch.FailedRows = failedCount;
                batch.SkippedRows = skippedCount;
                batch.Status = failedCount == 0
                    ? ImportBatchStatus.Completed.ToString()
                    : ImportBatchStatus.CompletedWithErrors.ToString();
                batch.CompletedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();
                await _transactionManager.CommitAsync();

                return new ImportSummaryDto
                {
                    BatchId = batch.Id,
                    Status = batch.Status,
                    TotalRows = batch.TotalRows,
                    SuccessRows = successCount,
                    FailedRows = failedCount,
                    SkippedRows = skippedCount,
                    DuplicateRows = skippedCount,
                    FailedRowDetails = failedDetails
                };
            }
            catch
            {
                await _transactionManager.RollbackAsync();
                batch.Status = ImportBatchStatus.Failed.ToString();
                throw;
            }
        }

        // ── Sales Channels ───────────────────────────────────────────────────────

        public async Task<PagedResult<SalesChannelDto>> GetChannelsAsync(PagedRequest request)
        {
            var result = await _channelRepository.GetPagedAsync(request);
            return new PagedResult<SalesChannelDto>
            {
                Items = result.Items.Select(MapChannelToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task<SalesChannelDto> CreateChannelAsync(CreateSalesChannelRequest request)
        {
            var slug = request.Name.ToLowerInvariant().Replace(" ", "-");
            var existing = await _channelRepository.GetBySlugAsync(slug);
            if (existing != null)
                throw new InvalidOperationException($"Sales channel '{request.Name}' already exists.");

            var channel = new SalesChannel
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Slug = slug,
                Description = request.Description,
                PlatformFeePercent = request.PlatformFeePercent,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _channelRepository.CreateAsync(channel);
            await _unitOfWork.SaveChangesAsync();
            return MapChannelToDto(channel);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static string? GetMappedValue(Dictionary<string, string> row, string? columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return null;
            return row.TryGetValue(columnName, out var value) ? value : null;
        }

        private static decimal ParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            var cleaned = value.Replace(",", "").Replace("Rp", "").Replace("IDR", "").Trim();
            return decimal.TryParse(cleaned, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var result) ? result : 0;
        }

        private static SalesChannelDto MapChannelToDto(SalesChannel c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            Description = c.Description,
            PlatformFeePercent = c.PlatformFeePercent,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        };

        private static CsvImportBatchDto MapBatchToDto(CsvImportBatch b) => new()
        {
            Id = b.Id,
            FileName = b.FileName,
            SalesChannelId = b.SalesChannelId,
            SalesChannelName = b.SalesChannel?.Name ?? string.Empty,
            WarehouseId = b.WarehouseId,
            WarehouseName = b.Warehouse?.Name ?? string.Empty,
            Status = b.Status,
            TotalRows = b.TotalRows,
            SuccessRows = b.SuccessRows,
            FailedRows = b.FailedRows,
            SkippedRows = b.SkippedRows,
            ErrorSummary = b.ErrorSummary,
            CreatedAt = b.CreatedAt,
            CompletedAt = b.CompletedAt
        };

        private static CsvImportBatchDetailDto MapBatchToDetailDto(CsvImportBatch b) => new()
        {
            Id = b.Id,
            FileName = b.FileName,
            SalesChannelId = b.SalesChannelId,
            SalesChannelName = b.SalesChannel?.Name ?? string.Empty,
            WarehouseId = b.WarehouseId,
            WarehouseName = b.Warehouse?.Name ?? string.Empty,
            Status = b.Status,
            TotalRows = b.TotalRows,
            SuccessRows = b.SuccessRows,
            FailedRows = b.FailedRows,
            SkippedRows = b.SkippedRows,
            ErrorSummary = b.ErrorSummary,
            CreatedAt = b.CreatedAt,
            CompletedAt = b.CompletedAt,
            Rows = b.Rows.Select(MapRowToDto).ToList()
        };

        private static CsvImportRowDto MapRowToDto(CsvImportRow r) => new()
        {
            Id = r.Id,
            RowNumber = r.RowNumber,
            OrderNumber = r.OrderNumber,
            Sku = r.Sku,
            ProductName = r.ProductName,
            Quantity = r.Quantity,
            UnitPrice = r.UnitPrice,
            TotalPrice = r.TotalPrice,
            PlatformFee = r.PlatformFee,
            OrderDate = r.OrderDate,
            Status = r.Status,
            MatchedProductVariantId = r.MatchedProductVariantId,
            MatchedSku = r.MatchedProductVariant?.Sku,
            ErrorMessage = r.ErrorMessage
        };
    }
}
