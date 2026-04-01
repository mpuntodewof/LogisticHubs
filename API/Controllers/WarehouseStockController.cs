using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Inventory;
using Application.UseCases.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/warehouse-stock")]
    [Authorize]
    public class WarehouseStockController : ControllerBase
    {
        private readonly WarehouseStockUseCase _warehouseStockUseCase;

        public WarehouseStockController(WarehouseStockUseCase warehouseStockUseCase)
        {
            _warehouseStockUseCase = warehouseStockUseCase;
        }

        /// <summary>Get all warehouse stock (paginated, optionally filtered).</summary>
        [HttpGet]
        [RequirePermission("inventory.read")]
        public async Task<ActionResult<PagedResult<WarehouseStockDto>>> GetAll([FromQuery] PagedRequest request, [FromQuery] Guid? warehouseId, [FromQuery] Guid? productVariantId)
        {
            var result = await _warehouseStockUseCase.GetPagedAsync(request, warehouseId, productVariantId);
            return Ok(result);
        }

        /// <summary>Get low stock items.</summary>
        [HttpGet("low-stock")]
        [RequirePermission("inventory.read")]
        public async Task<ActionResult<IEnumerable<WarehouseStockDto>>> GetLowStock([FromQuery] Guid? warehouseId)
        {
            var result = await _warehouseStockUseCase.GetLowStockAsync(warehouseId);
            return Ok(result);
        }

        /// <summary>Update stock settings (reorder point, min/max levels).</summary>
        [HttpPut("{id:guid}/settings")]
        [RequirePermission("inventory.update")]
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
    }
}
