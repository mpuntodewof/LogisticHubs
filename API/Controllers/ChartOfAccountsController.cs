using Asp.Versioning;
using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Finance;
using Application.UseCases.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/chart-of-accounts")]
    [Authorize]
    public class ChartOfAccountsController : ControllerBase
    {
        private readonly ChartOfAccountUseCase _useCase;

        public ChartOfAccountsController(ChartOfAccountUseCase useCase)
        {
            _useCase = useCase;
        }

        /// <summary>Get all chart of accounts (paginated).</summary>
        [HttpGet]
        [RequirePermission("chart-of-accounts.read")]
        public async Task<ActionResult<PagedResult<ChartOfAccountDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] string? accountType = null)
        {
            var result = await _useCase.GetPagedAsync(request, accountType);
            return Ok(result);
        }

        /// <summary>Get a chart of account by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("chart-of-accounts.read")]
        public async Task<ActionResult<ChartOfAccountDto>> GetById(Guid id)
        {
            var account = await _useCase.GetByIdAsync(id);
            if (account == null) return NotFound();
            return Ok(account);
        }

        /// <summary>Create a new chart of account.</summary>
        [HttpPost]
        [RequirePermission("chart-of-accounts.create")]
        public async Task<ActionResult<ChartOfAccountDto>> Create([FromBody] CreateChartOfAccountRequest request)
        {
            try
            {
                var account = await _useCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
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

        /// <summary>Update a chart of account.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("chart-of-accounts.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateChartOfAccountRequest request)
        {
            try
            {
                await _useCase.UpdateAsync(id, request);
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

        /// <summary>Delete a chart of account.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("chart-of-accounts.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _useCase.DeleteAsync(id);
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
