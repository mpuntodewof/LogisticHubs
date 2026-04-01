using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Drivers;
using Application.UseCases.Drivers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/drivers")]
    [Authorize]
    public class DriversController : ControllerBase
    {
        private readonly DriverUseCase _driverUseCase;

        public DriversController(DriverUseCase driverUseCase)
        {
            _driverUseCase = driverUseCase;
        }

        /// <summary>Get all drivers (paginated).</summary>
        [HttpGet]
        [RequirePermission("drivers.manage")]
        public async Task<ActionResult<PagedResult<DriverDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _driverUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a driver by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("drivers.manage")]
        public async Task<ActionResult<DriverDto>> GetById(Guid id)
        {
            var driver = await _driverUseCase.GetByIdAsync(id);
            if (driver == null) return NotFound();
            return Ok(driver);
        }

        /// <summary>Register a user as a driver.</summary>
        [HttpPost]
        [RequirePermission("drivers.manage")]
        public async Task<ActionResult<DriverDto>> Create([FromBody] CreateDriverRequest request)
        {
            try
            {
                var driver = await _driverUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = driver.Id }, driver);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a driver's details or status.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("drivers.manage")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDriverRequest request)
        {
            try
            {
                await _driverUseCase.UpdateAsync(id, request);
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

        /// <summary>Delete a driver.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("drivers.manage")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _driverUseCase.DeleteAsync(id);
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
