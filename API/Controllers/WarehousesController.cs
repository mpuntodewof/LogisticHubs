using Asp.Versioning;
using API.Filters;
using Domain.Constants;
using Application.DTOs.Common;
using Application.DTOs.Warehouses;
using Application.UseCases.Warehouses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/warehouses")]
    [Authorize]
    public class WarehousesController : ControllerBase
    {
        private readonly WarehouseUseCase _warehouseUseCase;

        public WarehousesController(WarehouseUseCase warehouseUseCase)
        {
            _warehouseUseCase = warehouseUseCase;
        }

        /// <summary>Get all warehouses (paginated).</summary>
        [HttpGet]
        [RequirePermission(Permissions.Warehouses.Manage)]
        public async Task<ActionResult<PagedResult<WarehouseDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _warehouseUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a warehouse by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission(Permissions.Warehouses.Manage)]
        public async Task<ActionResult<WarehouseDto>> GetById(Guid id)
        {
            var warehouse = await _warehouseUseCase.GetByIdAsync(id);
            if (warehouse == null) return NotFound();
            return Ok(warehouse);
        }

        /// <summary>Create a new warehouse.</summary>
        [HttpPost]
        [RequirePermission(Permissions.Warehouses.Manage)]
        public async Task<ActionResult<WarehouseDto>> Create([FromBody] CreateWarehouseRequest request)
        {
            try
            {
                var warehouse = await _warehouseUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = warehouse.Id }, warehouse);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a warehouse.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission(Permissions.Warehouses.Manage)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseRequest request)
        {
            try
            {
                await _warehouseUseCase.UpdateAsync(id, request);
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

        /// <summary>Delete a warehouse.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission(Permissions.Warehouses.Manage)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _warehouseUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
