using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.HRM;
using Application.UseCases.HRM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/employees")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeUseCase _employeeUseCase;

        public EmployeesController(EmployeeUseCase employeeUseCase)
        {
            _employeeUseCase = employeeUseCase;
        }

        /// <summary>Get all employees (paginated).</summary>
        [HttpGet]
        [RequirePermission("employees.read")]
        public async Task<ActionResult<PagedResult<EmployeeDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] Guid? departmentId = null,
            [FromQuery] string? status = null)
        {
            var result = await _employeeUseCase.GetPagedAsync(request, departmentId, status);
            return Ok(result);
        }

        /// <summary>Get an employee by ID (detail).</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("employees.read")]
        public async Task<ActionResult<EmployeeDetailDto>> GetById(Guid id)
        {
            var employee = await _employeeUseCase.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        /// <summary>Create a new employee.</summary>
        [HttpPost]
        [RequirePermission("employees.create")]
        public async Task<ActionResult<EmployeeDto>> Create([FromBody] CreateEmployeeRequest request)
        {
            try
            {
                var employee = await _employeeUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update an employee.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("employees.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request)
        {
            try
            {
                await _employeeUseCase.UpdateAsync(id, request);
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

        /// <summary>Delete an employee.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("employees.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _employeeUseCase.DeleteAsync(id);
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
