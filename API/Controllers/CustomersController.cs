using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Customers;
using Application.UseCases.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/customers")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerUseCase _customerUseCase;
        private readonly CustomerAddressUseCase _addressUseCase;

        public CustomersController(CustomerUseCase customerUseCase, CustomerAddressUseCase addressUseCase)
        {
            _customerUseCase = customerUseCase;
            _addressUseCase = addressUseCase;
        }

        /// <summary>Get all customers (paginated).</summary>
        [HttpGet]
        [RequirePermission("customers.read")]
        public async Task<ActionResult<PagedResult<CustomerDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _customerUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a customer by ID (with addresses).</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("customers.read")]
        public async Task<ActionResult<CustomerDetailDto>> GetById(Guid id)
        {
            var customer = await _customerUseCase.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        /// <summary>Create a new customer.</summary>
        [HttpPost]
        [RequirePermission("customers.create")]
        public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerRequest request)
        {
            try
            {
                var customer = await _customerUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a customer.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("customers.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request)
        {
            try
            {
                await _customerUseCase.UpdateAsync(id, request);
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

        /// <summary>Delete a customer.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("customers.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _customerUseCase.DeleteAsync(id);
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

        // ── Address Endpoints ────────────────────────────────────────────────────

        /// <summary>Get all addresses for a customer.</summary>
        [HttpGet("{id:guid}/addresses")]
        [RequirePermission("customers.read")]
        public async Task<ActionResult<IEnumerable<CustomerAddressDto>>> GetAddresses(Guid id)
        {
            var addresses = await _addressUseCase.GetByCustomerIdAsync(id);
            return Ok(addresses);
        }

        /// <summary>Create a new address for a customer.</summary>
        [HttpPost("{id:guid}/addresses")]
        [RequirePermission("customers.create")]
        public async Task<ActionResult<CustomerAddressDto>> CreateAddress(Guid id, [FromBody] CreateCustomerAddressRequest request)
        {
            try
            {
                var address = await _addressUseCase.CreateAsync(id, request);
                return CreatedAtAction(nameof(GetById), new { id }, address);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a customer address.</summary>
        [HttpPut("{id:guid}/addresses/{addressId:guid}")]
        [RequirePermission("customers.update")]
        public async Task<IActionResult> UpdateAddress(Guid id, Guid addressId, [FromBody] UpdateCustomerAddressRequest request)
        {
            try
            {
                await _addressUseCase.UpdateAsync(addressId, request);
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

        /// <summary>Delete a customer address.</summary>
        [HttpDelete("{id:guid}/addresses/{addressId:guid}")]
        [RequirePermission("customers.delete")]
        public async Task<IActionResult> DeleteAddress(Guid id, Guid addressId)
        {
            try
            {
                await _addressUseCase.DeleteAsync(addressId);
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
