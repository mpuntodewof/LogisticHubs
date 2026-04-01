using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.UnitsOfMeasure;
using Application.UseCases.UnitsOfMeasure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/units-of-measure")]
    [Authorize]
    public class UnitsOfMeasureController : ControllerBase
    {
        private readonly UnitOfMeasureUseCase _unitOfMeasureUseCase;

        public UnitsOfMeasureController(UnitOfMeasureUseCase unitOfMeasureUseCase)
        {
            _unitOfMeasureUseCase = unitOfMeasureUseCase;
        }

        /// <summary>Get all units of measure (paginated).</summary>
        [HttpGet]
        [RequirePermission("units.read")]
        public async Task<ActionResult<PagedResult<UnitOfMeasureDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _unitOfMeasureUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a unit of measure by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("units.read")]
        public async Task<ActionResult<UnitOfMeasureDto>> GetById(Guid id)
        {
            var unit = await _unitOfMeasureUseCase.GetByIdAsync(id);
            if (unit == null) return NotFound();
            return Ok(unit);
        }

        /// <summary>Create a new unit of measure.</summary>
        [HttpPost]
        [RequirePermission("units.create")]
        public async Task<ActionResult<UnitOfMeasureDto>> Create([FromBody] CreateUnitOfMeasureRequest request)
        {
            try
            {
                var unit = await _unitOfMeasureUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = unit.Id }, unit);
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

        /// <summary>Update a unit of measure.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("units.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUnitOfMeasureRequest request)
        {
            try
            {
                await _unitOfMeasureUseCase.UpdateAsync(id, request);
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

        /// <summary>Delete a unit of measure.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("units.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _unitOfMeasureUseCase.DeleteAsync(id);
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

        /// <summary>Get conversions for a unit of measure.</summary>
        [HttpGet("{id:guid}/conversions")]
        [RequirePermission("units.read")]
        public async Task<ActionResult<IEnumerable<UnitConversionDto>>> GetConversions(Guid id)
        {
            try
            {
                var result = await _unitOfMeasureUseCase.GetConversionsAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Create a unit conversion.</summary>
        [HttpPost("conversions")]
        [RequirePermission("units.create")]
        public async Task<ActionResult<UnitConversionDto>> CreateConversion([FromBody] CreateUnitConversionRequest request)
        {
            try
            {
                var conversion = await _unitOfMeasureUseCase.CreateConversionAsync(request);
                return Ok(conversion);
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

        /// <summary>Update a unit conversion.</summary>
        [HttpPut("conversions/{id:guid}")]
        [RequirePermission("units.update")]
        public async Task<IActionResult> UpdateConversion(Guid id, [FromBody] UpdateUnitConversionRequest request)
        {
            try
            {
                await _unitOfMeasureUseCase.UpdateConversionAsync(id, request);
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

        /// <summary>Delete a unit conversion.</summary>
        [HttpDelete("conversions/{id:guid}")]
        [RequirePermission("units.delete")]
        public async Task<IActionResult> DeleteConversion(Guid id)
        {
            try
            {
                await _unitOfMeasureUseCase.DeleteConversionAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
