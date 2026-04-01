using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Vehicles;
using Application.UseCases.Vehicles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private readonly VehicleUseCase _vehicleUseCase;

        public VehiclesController(VehicleUseCase vehicleUseCase)
        {
            _vehicleUseCase = vehicleUseCase;
        }

        /// <summary>Get all vehicles (paginated).</summary>
        [HttpGet]
        [RequirePermission("vehicles.manage")]
        public async Task<ActionResult<PagedResult<VehicleDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _vehicleUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a vehicle by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("vehicles.manage")]
        public async Task<ActionResult<VehicleDto>> GetById(Guid id)
        {
            var vehicle = await _vehicleUseCase.GetByIdAsync(id);
            if (vehicle == null) return NotFound();
            return Ok(vehicle);
        }

        /// <summary>Register a new vehicle.</summary>
        [HttpPost]
        [RequirePermission("vehicles.manage")]
        public async Task<ActionResult<VehicleDto>> Create([FromBody] CreateVehicleRequest request)
        {
            try
            {
                var vehicle = await _vehicleUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a vehicle's details or status.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("vehicles.manage")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehicleRequest request)
        {
            try
            {
                await _vehicleUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Delete a vehicle.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("vehicles.manage")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _vehicleUseCase.DeleteAsync(id);
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
