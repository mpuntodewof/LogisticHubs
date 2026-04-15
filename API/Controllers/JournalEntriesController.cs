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
    [Route("api/v{version:apiVersion}/journal-entries")]
    [Authorize]
    public class JournalEntriesController : ControllerBase
    {
        private readonly JournalEntryUseCase _useCase;

        public JournalEntriesController(JournalEntryUseCase useCase)
        {
            _useCase = useCase;
        }

        /// <summary>Get all journal entries (paginated).</summary>
        [HttpGet]
        [RequirePermission("journal-entries.read")]
        public async Task<ActionResult<PagedResult<JournalEntryDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] string? status = null)
        {
            var result = await _useCase.GetPagedAsync(request, status);
            return Ok(result);
        }

        /// <summary>Get a journal entry by ID with lines.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("journal-entries.read")]
        public async Task<ActionResult<JournalEntryDetailDto>> GetById(Guid id)
        {
            var entry = await _useCase.GetByIdAsync(id);
            if (entry == null) return NotFound();
            return Ok(entry);
        }

        /// <summary>Create a new journal entry.</summary>
        [HttpPost]
        [RequirePermission("journal-entries.create")]
        public async Task<ActionResult<JournalEntryDetailDto>> Create([FromBody] CreateJournalEntryRequest request)
        {
            try
            {
                var entry = await _useCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Post a draft journal entry.</summary>
        [HttpPost("{id:guid}/post")]
        [RequirePermission("journal-entries.post")]
        public async Task<IActionResult> Post(Guid id)
        {
            try
            {
                await _useCase.PostAsync(id);
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

        /// <summary>Void a posted journal entry.</summary>
        [HttpPost("{id:guid}/void")]
        [RequirePermission("journal-entries.void")]
        public async Task<IActionResult> Void(Guid id, [FromBody] VoidJournalEntryRequest request)
        {
            try
            {
                await _useCase.VoidAsync(id, request);
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

        /// <summary>Delete a draft journal entry.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("journal-entries.delete")]
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
