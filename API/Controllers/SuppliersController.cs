using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Purchase;
using Application.UseCases.Purchase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/suppliers")]
    [Authorize]
    public class SuppliersController : ControllerBase
    {
        private readonly SupplierUseCase _supplierUseCase;

        public SuppliersController(SupplierUseCase supplierUseCase)
        {
            _supplierUseCase = supplierUseCase;
        }

        /// <summary>Get all suppliers (paginated).</summary>
        [HttpGet]
        [RequirePermission("suppliers.read")]
        public async Task<ActionResult<PagedResult<SupplierDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] bool? isActive)
        {
            var result = await _supplierUseCase.GetPagedAsync(request, isActive);
            return Ok(result);
        }

        /// <summary>Get a supplier by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("suppliers.read")]
        public async Task<ActionResult<SupplierDetailDto>> GetById(Guid id)
        {
            var supplier = await _supplierUseCase.GetByIdAsync(id);
            if (supplier == null) return NotFound();
            return Ok(supplier);
        }

        /// <summary>Create a new supplier.</summary>
        [HttpPost]
        [RequirePermission("suppliers.create")]
        public async Task<ActionResult<SupplierDto>> Create([FromBody] CreateSupplierRequest request)
        {
            try
            {
                var supplier = await _supplierUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a supplier.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("suppliers.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupplierRequest request)
        {
            try
            {
                await _supplierUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Delete a supplier.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("suppliers.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _supplierUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
