using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Purchase;
using Application.UseCases.Purchase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/purchase-orders")]
    [Authorize]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly PurchaseOrderUseCase _purchaseOrderUseCase;

        public PurchaseOrdersController(PurchaseOrderUseCase purchaseOrderUseCase)
        {
            _purchaseOrderUseCase = purchaseOrderUseCase;
        }

        /// <summary>Get all purchase orders (paginated).</summary>
        [HttpGet]
        [RequirePermission("purchase-orders.read")]
        public async Task<ActionResult<PagedResult<PurchaseOrderDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] string? status,
            [FromQuery] Guid? supplierId,
            [FromQuery] Guid? warehouseId)
        {
            var result = await _purchaseOrderUseCase.GetPagedAsync(request, status, supplierId, warehouseId);
            return Ok(result);
        }

        /// <summary>Get a purchase order by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("purchase-orders.read")]
        public async Task<ActionResult<PurchaseOrderDetailDto>> GetById(Guid id)
        {
            var order = await _purchaseOrderUseCase.GetByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        /// <summary>Create a new purchase order.</summary>
        [HttpPost]
        [RequirePermission("purchase-orders.create")]
        public async Task<ActionResult<PurchaseOrderDto>> Create([FromBody] CreatePurchaseOrderRequest request)
        {
            try
            {
                var order = await _purchaseOrderUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a purchase order.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("purchase-orders.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePurchaseOrderRequest request)
        {
            try
            {
                await _purchaseOrderUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Delete a purchase order.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("purchase-orders.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _purchaseOrderUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Submit a purchase order for approval.</summary>
        [HttpPost("{id:guid}/submit")]
        [RequirePermission("purchase-orders.submit")]
        public async Task<IActionResult> Submit(Guid id)
        {
            try
            {
                await _purchaseOrderUseCase.SubmitAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Approve a purchase order.</summary>
        [HttpPost("{id:guid}/approve")]
        [RequirePermission("purchase-orders.approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            try
            {
                await _purchaseOrderUseCase.ApproveAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Cancel a purchase order.</summary>
        [HttpPost("{id:guid}/cancel")]
        [RequirePermission("purchase-orders.cancel")]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelPurchaseOrderRequest request)
        {
            try
            {
                await _purchaseOrderUseCase.CancelAsync(id, request.Reason);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }

    public class CancelPurchaseOrderRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}
