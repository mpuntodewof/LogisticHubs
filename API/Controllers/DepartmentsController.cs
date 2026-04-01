using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.HRM;
using Application.UseCases.HRM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/departments")]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly DepartmentUseCase _departmentUseCase;

        public DepartmentsController(DepartmentUseCase departmentUseCase)
        {
            _departmentUseCase = departmentUseCase;
        }

        /// <summary>Get all departments (paginated).</summary>
        [HttpGet]
        [RequirePermission("departments.read")]
        public async Task<ActionResult<PagedResult<DepartmentDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _departmentUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get all departments (for dropdowns).</summary>
        [HttpGet("all")]
        [RequirePermission("departments.read")]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAllList()
        {
            var result = await _departmentUseCase.GetAllAsync();
            return Ok(result);
        }

        /// <summary>Get a department by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("departments.read")]
        public async Task<ActionResult<DepartmentDto>> GetById(Guid id)
        {
            var department = await _departmentUseCase.GetByIdAsync(id);
            if (department == null) return NotFound();
            return Ok(department);
        }

        /// <summary>Create a new department.</summary>
        [HttpPost]
        [RequirePermission("departments.create")]
        public async Task<ActionResult<DepartmentDto>> Create([FromBody] CreateDepartmentRequest request)
        {
            try
            {
                var department = await _departmentUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
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

        /// <summary>Update a department.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("departments.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentRequest request)
        {
            try
            {
                await _departmentUseCase.UpdateAsync(id, request);
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

        /// <summary>Delete a department.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("departments.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _departmentUseCase.DeleteAsync(id);
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
