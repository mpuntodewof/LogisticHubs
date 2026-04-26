using Asp.Versioning;
using API.Filters;
using Domain.Constants;
using Application.DTOs.Common;
using Application.DTOs.Inventory;
using Application.UseCases.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/warehouse-stock")]
    [Authorize]
    public class WarehouseStockController : ControllerBase
    {
        private readonly WarehouseStockUseCase _warehouseStockUseCase;
        private readonly StockReconciliationUseCase _reconciliationUseCase;

        public WarehouseStockController(
            WarehouseStockUseCase warehouseStockUseCase,
            StockReconciliationUseCase reconciliationUseCase)
        {
            _warehouseStockUseCase = warehouseStockUseCase;
            _reconciliationUseCase = reconciliationUseCase;
        }

        /// <summary>Get all warehouse stock (paginated, optionally filtered).</summary>
        [HttpGet]
        [RequirePermission(Permissions.Inventory.Read)]
        public async Task<ActionResult<PagedResult<WarehouseStockDto>>> GetAll([FromQuery] PagedRequest request, [FromQuery] Guid? warehouseId, [FromQuery] Guid? productVariantId)
        {
            var result = await _warehouseStockUseCase.GetPagedAsync(request, warehouseId, productVariantId);
            return Ok(result);
        }

        /// <summary>Get low stock items.</summary>
        [HttpGet("low-stock")]
        [RequirePermission(Permissions.Inventory.Read)]
        public async Task<ActionResult<IEnumerable<WarehouseStockDto>>> GetLowStock([FromQuery] Guid? warehouseId)
        {
            var result = await _warehouseStockUseCase.GetLowStockAsync(warehouseId);
            return Ok(result);
        }

        /// <summary>Update stock settings (reorder point, min/max levels).</summary>
        [HttpPut("{id:guid}/settings")]
        [RequirePermission(Permissions.Inventory.Update)]
        public async Task<IActionResult> UpdateSettings(Guid id, [FromBody] UpdateWarehouseStockRequest request)
        {
            try
            {
                await _warehouseStockUseCase.UpdateSettingsAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Reconcile stock with physical counts.</summary>
        [HttpPost("reconcile")]
        [RequirePermission(Permissions.Inventory.Update)]
        public async Task<ActionResult<StockReconciliationResult>> Reconcile([FromBody] StockReconciliationRequest request)
        {
            var result = await _reconciliationUseCase.ReconcileAsync(request);
            return Ok(result);
        }
    }
}
