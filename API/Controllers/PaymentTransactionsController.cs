using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.PaymentGateway;
using Application.UseCases.PaymentGateway;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/payment-transactions")]
    [Authorize]
    public class PaymentTransactionsController : ControllerBase
    {
        private readonly PaymentTransactionUseCase _useCase;

        public PaymentTransactionsController(PaymentTransactionUseCase useCase)
        {
            _useCase = useCase;
        }

        /// <summary>Get all payment transactions (paginated).</summary>
        [HttpGet]
        [RequirePermission("payment-transactions.read")]
        public async Task<ActionResult<PagedResult<PaymentTransactionDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] Guid? salesOrderPaymentId,
            [FromQuery] string? status)
        {
            var result = await _useCase.GetPagedAsync(request, salesOrderPaymentId, status);
            return Ok(result);
        }

        /// <summary>Get a payment transaction by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("payment-transactions.read")]
        public async Task<ActionResult<PaymentTransactionDetailDto>> GetById(Guid id)
        {
            var transaction = await _useCase.GetByIdAsync(id);
            if (transaction == null) return NotFound();
            return Ok(transaction);
        }

        /// <summary>Initiate a payment transaction.</summary>
        [HttpPost("initiate")]
        [RequirePermission("payment-transactions.create")]
        public async Task<ActionResult<PaymentTransactionDto>> Initiate([FromBody] InitiatePaymentRequest request)
        {
            try
            {
                var transaction = await _useCase.InitiatePaymentAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
