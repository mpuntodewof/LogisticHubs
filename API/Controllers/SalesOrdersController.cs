using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Sales;
using Application.UseCases.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/sales-orders")]
    [Authorize]
    public class SalesOrdersController : ControllerBase
    {
        private readonly SalesOrderUseCase _salesOrderUseCase;

        public SalesOrdersController(SalesOrderUseCase salesOrderUseCase)
        {
            _salesOrderUseCase = salesOrderUseCase;
        }

        /// <summary>Get all sales orders (paginated).</summary>
        [HttpGet]
        [RequirePermission("sales-orders.read")]
        public async Task<ActionResult<PagedResult<SalesOrderDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] string? status,
            [FromQuery] Guid? customerId,
            [FromQuery] Guid? branchId)
        {
            var result = await _salesOrderUseCase.GetPagedAsync(request, status, customerId, branchId);
            return Ok(result);
        }

        /// <summary>Get a sales order by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("sales-orders.read")]
        public async Task<ActionResult<SalesOrderDetailDto>> GetById(Guid id)
        {
            var order = await _salesOrderUseCase.GetByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        /// <summary>Create a new sales order.</summary>
        [HttpPost]
        [RequirePermission("sales-orders.create")]
        public async Task<ActionResult<SalesOrderDto>> Create([FromBody] CreateSalesOrderRequest request)
        {
            try
            {
                var order = await _salesOrderUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a sales order.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("sales-orders.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSalesOrderRequest request)
        {
            try
            {
                await _salesOrderUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Delete a sales order.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("sales-orders.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _salesOrderUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Confirm a sales order.</summary>
        [HttpPost("{id:guid}/confirm")]
        [RequirePermission("sales-orders.confirm")]
        public async Task<IActionResult> Confirm(Guid id)
        {
            try
            {
                await _salesOrderUseCase.ConfirmAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Cancel a sales order.</summary>
        [HttpPost("{id:guid}/cancel")]
        [RequirePermission("sales-orders.cancel")]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelOrderRequest request)
        {
            try
            {
                await _salesOrderUseCase.CancelAsync(id, request.Reason);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Add a payment to a sales order.</summary>
        [HttpPost("{id:guid}/payments")]
        [RequirePermission("sales-orders.pay")]
        public async Task<ActionResult<SalesOrderPaymentDto>> AddPayment(Guid id, [FromBody] CreateSalesOrderPaymentRequest request)
        {
            try
            {
                var payment = await _salesOrderUseCase.AddPaymentAsync(id, request);
                return Ok(payment);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Get payments for a sales order.</summary>
        [HttpGet("{id:guid}/payments")]
        [RequirePermission("sales-orders.read")]
        public async Task<ActionResult<IEnumerable<SalesOrderPaymentDto>>> GetPayments(Guid id)
        {
            var payments = await _salesOrderUseCase.GetPaymentsAsync(id);
            return Ok(payments);
        }
    }

    public class CancelOrderRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}
