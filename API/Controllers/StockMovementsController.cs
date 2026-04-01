using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Inventory;
using Application.UseCases.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/stock-movements")]
    [Authorize]
    public class StockMovementsController : ControllerBase
    {
        private readonly StockMovementUseCase _stockMovementUseCase;

        public StockMovementsController(StockMovementUseCase stockMovementUseCase)
        {
            _stockMovementUseCase = stockMovementUseCase;
        }

        /// <summary>Get all stock movements (paginated, optionally filtered).</summary>
        [HttpGet]
        [RequirePermission("inventory.read")]
        public async Task<ActionResult<PagedResult<StockMovementDto>>> GetAll([FromQuery] PagedRequest request, [FromQuery] Guid? warehouseId, [FromQuery] Guid? productVariantId, [FromQuery] string? movementType)
        {
            var result = await _stockMovementUseCase.GetPagedAsync(request, warehouseId, productVariantId, movementType);
            return Ok(result);
        }

        /// <summary>Get a stock movement by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("inventory.read")]
        public async Task<ActionResult<StockMovementDto>> GetById(Guid id)
        {
            var movement = await _stockMovementUseCase.GetByIdAsync(id);
            if (movement == null) return NotFound();
            return Ok(movement);
        }

        /// <summary>Create a new stock movement.</summary>
        [HttpPost]
        [RequirePermission("inventory.create")]
        public async Task<ActionResult<StockMovementDto>> Create([FromBody] CreateStockMovementRequest request)
        {
            try
            {
                var movement = await _stockMovementUseCase.CreateMovementAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = movement.Id }, movement);
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

        /// <summary>Create a stock transfer between warehouses.</summary>
        [HttpPost("transfer")]
        [RequirePermission("inventory.transfer")]
        public async Task<ActionResult<StockMovementDto>> CreateTransfer([FromBody] CreateStockTransferRequest request)
        {
            try
            {
                var movement = await _stockMovementUseCase.CreateTransferAsync(request);
                return Ok(movement);
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
