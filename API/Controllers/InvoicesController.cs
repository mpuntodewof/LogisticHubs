using Asp.Versioning;
using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Tax;
using Application.UseCases.Tax;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/invoices")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly InvoiceUseCase _invoiceUseCase;

        public InvoicesController(InvoiceUseCase invoiceUseCase)
        {
            _invoiceUseCase = invoiceUseCase;
        }

        /// <summary>Get all invoices (paginated).</summary>
        [HttpGet]
        [RequirePermission("invoices.read")]
        public async Task<ActionResult<PagedResult<InvoiceDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] string? status)
        {
            var result = await _invoiceUseCase.GetPagedAsync(request, status);
            return Ok(result);
        }

        /// <summary>Get an invoice by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("invoices.read")]
        public async Task<ActionResult<InvoiceDetailDto>> GetById(Guid id)
        {
            var invoice = await _invoiceUseCase.GetByIdAsync(id);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }

        /// <summary>Create a new invoice.</summary>
        [HttpPost]
        [RequirePermission("invoices.create")]
        public async Task<ActionResult<InvoiceDto>> Create([FromBody] CreateInvoiceRequest request)
        {
            try
            {
                var invoice = await _invoiceUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Issue an invoice.</summary>
        [HttpPost("{id:guid}/issue")]
        [RequirePermission("invoices.issue")]
        public async Task<IActionResult> Issue(Guid id)
        {
            try
            {
                await _invoiceUseCase.IssueAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Assign a tax invoice number.</summary>
        [HttpPost("{id:guid}/assign-tax-number")]
        [RequirePermission("invoices.assign-tax-number")]
        public async Task<IActionResult> AssignTaxInvoiceNumber(Guid id, [FromBody] AssignTaxInvoiceNumberRequest request)
        {
            try
            {
                await _invoiceUseCase.AssignTaxInvoiceNumberAsync(id, request);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Mark an invoice as paid.</summary>
        [HttpPost("{id:guid}/mark-paid")]
        [RequirePermission("invoices.pay")]
        public async Task<IActionResult> MarkPaid(Guid id)
        {
            try
            {
                await _invoiceUseCase.MarkPaidAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Cancel an invoice.</summary>
        [HttpPost("{id:guid}/cancel")]
        [RequirePermission("invoices.cancel")]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelInvoiceRequest request)
        {
            try
            {
                await _invoiceUseCase.CancelAsync(id, request.Reason);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Delete a draft invoice.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("invoices.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _invoiceUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }

    public class CancelInvoiceRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}
