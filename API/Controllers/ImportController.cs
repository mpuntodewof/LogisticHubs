using Asp.Versioning;
using API.Filters;
using Domain.Constants;
using Application.DTOs.Common;
using Application.DTOs.Import;
using Application.UseCases.Import;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/import")]
    [Authorize]
    public class ImportController : ControllerBase
    {
        private readonly CsvImportUseCase _importUseCase;

        public ImportController(CsvImportUseCase importUseCase)
        {
            _importUseCase = importUseCase;
        }

        // â”€â”€ Sales Channels â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [HttpGet("channels")]
        [RequirePermission(Permissions.Inventory.Read)]
        public async Task<ActionResult<PagedResult<SalesChannelDto>>> GetChannels([FromQuery] PagedRequest request)
        {
            var result = await _importUseCase.GetChannelsAsync(request);
            return Ok(result);
        }

        [HttpPost("channels")]
        [RequirePermission(Permissions.Inventory.Create)]
        public async Task<ActionResult<SalesChannelDto>> CreateChannel([FromBody] CreateSalesChannelRequest request)
        {
            var channel = await _importUseCase.CreateChannelAsync(request);
            return CreatedAtAction(null, null, channel);
        }

        [HttpPut("channels/{id:guid}")]
        [RequirePermission(Permissions.Inventory.Update)]
        public async Task<ActionResult<SalesChannelDto>> UpdateChannel(Guid id, [FromBody] UpdateSalesChannelRequest request)
        {
            try
            {
                var channel = await _importUseCase.UpdateChannelAsync(id, request);
                return Ok(channel);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        // â”€â”€ CSV Import â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [HttpPost("csv/preview")]
        [RequirePermission(Permissions.Inventory.Create)]
        public ActionResult<List<string>> PreviewCsvHeaders(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded." });

            using var stream = file.OpenReadStream();
            var headers = _importUseCase.GetCsvHeaders(stream);
            return Ok(headers);
        }

        [HttpPost("csv/process")]
        [RequirePermission(Permissions.Inventory.Create)]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB max
        public async Task<ActionResult<ImportSummaryDto>> ProcessCsvImport(
            IFormFile file,
            [FromForm] Guid salesChannelId,
            [FromForm] Guid warehouseId,
            [FromForm] string orderNumberColumn,
            [FromForm] string skuColumn,
            [FromForm] string quantityColumn,
            [FromForm] string unitPriceColumn,
            [FromForm] string? totalPriceColumn,
            [FromForm] string? productNameColumn,
            [FromForm] string? orderDateColumn,
            [FromForm] string? platformFeeColumn)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded." });

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { error = "Only CSV files are accepted." });

            var request = new StartImportRequest
            {
                SalesChannelId = salesChannelId,
                WarehouseId = warehouseId,
                ColumnMapping = new CsvColumnMapping
                {
                    OrderNumberColumn = orderNumberColumn,
                    SkuColumn = skuColumn,
                    QuantityColumn = quantityColumn,
                    UnitPriceColumn = unitPriceColumn,
                    TotalPriceColumn = totalPriceColumn,
                    ProductNameColumn = productNameColumn,
                    OrderDateColumn = orderDateColumn,
                    PlatformFeeColumn = platformFeeColumn
                }
            };

            using var stream = file.OpenReadStream();
            var summary = await _importUseCase.ProcessImportAsync(stream, file.FileName, request);
            return Ok(summary);
        }

        // â”€â”€ Initial Stock Import â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [HttpPost("csv/initial-stock")]
        [RequirePermission(Permissions.Inventory.Create)]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<ActionResult<InitialStockResultDto>> ProcessInitialStockImport(
            IFormFile file,
            [FromForm] Guid warehouseId,
            [FromForm] string skuColumn,
            [FromForm] string quantityColumn)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded." });

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { error = "Only CSV files are accepted." });

            var request = new StartInitialStockRequest
            {
                WarehouseId = warehouseId,
                ColumnMapping = new InitialStockColumnMapping
                {
                    SkuColumn = skuColumn,
                    QuantityColumn = quantityColumn
                }
            };

            using var stream = file.OpenReadStream();
            var result = await _importUseCase.ProcessInitialStockAsync(stream, file.FileName, request);
            return Ok(result);
        }

        // â”€â”€ Import History â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [HttpGet("batches")]
        [RequirePermission(Permissions.Inventory.Read)]
        public async Task<ActionResult<PagedResult<CsvImportBatchDto>>> GetBatches([FromQuery] PagedRequest request)
        {
            var result = await _importUseCase.GetBatchesAsync(request);
            return Ok(result);
        }

        [HttpGet("batches/{id:guid}")]
        [RequirePermission(Permissions.Inventory.Read)]
        public async Task<ActionResult<CsvImportBatchDetailDto>> GetBatchDetail(Guid id)
        {
            var batch = await _importUseCase.GetBatchDetailAsync(id);
            if (batch == null) return NotFound(new { error = "Import batch not found." });
            return Ok(batch);
        }

        [HttpDelete("batches/{id:guid}")]
        [RequirePermission(Permissions.Inventory.Update)]
        public async Task<IActionResult> DeleteBatch(Guid id)
        {
            try
            {
                await _importUseCase.DeleteBatchAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
