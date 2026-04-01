using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Customers;
using Application.UseCases.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/customer-groups")]
    [Authorize]
    public class CustomerGroupsController : ControllerBase
    {
        private readonly CustomerGroupUseCase _customerGroupUseCase;

        public CustomerGroupsController(CustomerGroupUseCase customerGroupUseCase)
        {
            _customerGroupUseCase = customerGroupUseCase;
        }

        /// <summary>Get all customer groups (paginated).</summary>
        [HttpGet]
        [RequirePermission("customer-groups.read")]
        public async Task<ActionResult<PagedResult<CustomerGroupDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _customerGroupUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a customer group by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("customer-groups.read")]
        public async Task<ActionResult<CustomerGroupDto>> GetById(Guid id)
        {
            var customerGroup = await _customerGroupUseCase.GetByIdAsync(id);
            if (customerGroup == null) return NotFound();
            return Ok(customerGroup);
        }

        /// <summary>Create a new customer group.</summary>
        [HttpPost]
        [RequirePermission("customer-groups.create")]
        public async Task<ActionResult<CustomerGroupDto>> Create([FromBody] CreateCustomerGroupRequest request)
        {
            try
            {
                var customerGroup = await _customerGroupUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = customerGroup.Id }, customerGroup);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a customer group.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("customer-groups.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerGroupRequest request)
        {
            try
            {
                await _customerGroupUseCase.UpdateAsync(id, request);
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

        /// <summary>Delete a customer group.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("customer-groups.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _customerGroupUseCase.DeleteAsync(id);
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
