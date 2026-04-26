using Asp.Versioning;
using API.Filters;
using Domain.Constants;
using Application.DTOs.Common;
using Application.DTOs.Tax;
using Application.UseCases.Tax;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/tax-rates")]
    [Authorize]
    public class TaxRatesController : ControllerBase
    {
        private readonly TaxRateUseCase _taxRateUseCase;

        public TaxRatesController(TaxRateUseCase taxRateUseCase)
        {
            _taxRateUseCase = taxRateUseCase;
        }

        /// <summary>Get all tax rates (paginated).</summary>
        [HttpGet]
        [RequirePermission(Permissions.TaxRates.Read)]
        public async Task<ActionResult<PagedResult<TaxRateDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _taxRateUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a tax rate by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission(Permissions.TaxRates.Read)]
        public async Task<ActionResult<TaxRateDto>> GetById(Guid id)
        {
            var taxRate = await _taxRateUseCase.GetByIdAsync(id);
            if (taxRate == null) return NotFound();
            return Ok(taxRate);
        }

        /// <summary>Create a new tax rate.</summary>
        [HttpPost]
        [RequirePermission(Permissions.TaxRates.Create)]
        public async Task<ActionResult<TaxRateDto>> Create([FromBody] CreateTaxRateRequest request)
        {
            try
            {
                var taxRate = await _taxRateUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = taxRate.Id }, taxRate);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a tax rate.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission(Permissions.TaxRates.Update)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaxRateRequest request)
        {
            try
            {
                await _taxRateUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Delete a tax rate.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission(Permissions.TaxRates.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _taxRateUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Assign a tax rate to a product.</summary>
        [HttpPost("{id:guid}/products/{productId:guid}")]
        [RequirePermission(Permissions.TaxRates.Assign)]
        public async Task<IActionResult> AssignToProduct(Guid id, Guid productId)
        {
            try
            {
                await _taxRateUseCase.AssignToProductAsync(id, productId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Remove a tax rate from a product.</summary>
        [HttpDelete("{id:guid}/products/{productId:guid}")]
        [RequirePermission(Permissions.TaxRates.Assign)]
        public async Task<IActionResult> RemoveFromProduct(Guid id, Guid productId)
        {
            try
            {
                await _taxRateUseCase.RemoveFromProductAsync(id, productId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Get tax rates for a product.</summary>
        [HttpGet("by-product/{productId:guid}")]
        [RequirePermission(Permissions.TaxRates.Read)]
        public async Task<ActionResult<IEnumerable<TaxRateDto>>> GetByProduct(Guid productId)
        {
            var taxRates = await _taxRateUseCase.GetByProductIdAsync(productId);
            return Ok(taxRates);
        }
    }
}
