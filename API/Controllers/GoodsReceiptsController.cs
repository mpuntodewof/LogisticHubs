using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Purchase;
using Application.UseCases.Purchase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/goods-receipts")]
    [Authorize]
    public class GoodsReceiptsController : ControllerBase
    {
        private readonly GoodsReceiptUseCase _goodsReceiptUseCase;

        public GoodsReceiptsController(GoodsReceiptUseCase goodsReceiptUseCase)
        {
            _goodsReceiptUseCase = goodsReceiptUseCase;
        }

        /// <summary>Get all goods receipts (paginated).</summary>
        [HttpGet]
        [RequirePermission("goods-receipts.read")]
        public async Task<ActionResult<PagedResult<GoodsReceiptDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] Guid? purchaseOrderId)
        {
            var result = await _goodsReceiptUseCase.GetPagedAsync(request, purchaseOrderId);
            return Ok(result);
        }

        /// <summary>Get a goods receipt by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("goods-receipts.read")]
        public async Task<ActionResult<GoodsReceiptDetailDto>> GetById(Guid id)
        {
            var receipt = await _goodsReceiptUseCase.GetByIdAsync(id);
            if (receipt == null) return NotFound();
            return Ok(receipt);
        }

        /// <summary>Create a new goods receipt.</summary>
        [HttpPost]
        [RequirePermission("goods-receipts.create")]
        public async Task<ActionResult<GoodsReceiptDto>> Create([FromBody] CreateGoodsReceiptRequest request)
        {
            try
            {
                var receipt = await _goodsReceiptUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = receipt.Id }, receipt);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Confirm a goods receipt (updates stock).</summary>
        [HttpPost("{id:guid}/confirm")]
        [RequirePermission("goods-receipts.confirm")]
        public async Task<IActionResult> Confirm(Guid id)
        {
            try
            {
                await _goodsReceiptUseCase.ConfirmAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Delete a goods receipt.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("goods-receipts.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _goodsReceiptUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
