using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Storefront;
using Application.UseCases.Storefront;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/admin/pages")]
    [Authorize]
    public class PagesController : ControllerBase
    {
        private readonly PageUseCase _pageUseCase;

        public PagesController(PageUseCase pageUseCase)
        {
            _pageUseCase = pageUseCase;
        }

        /// <summary>Get all pages (paginated).</summary>
        [HttpGet]
        [RequirePermission("pages.read")]
        public async Task<ActionResult<PagedResult<PageDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] string? status = null)
        {
            var result = await _pageUseCase.GetPagedAsync(request, status);
            return Ok(result);
        }

        /// <summary>Get a page by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("pages.read")]
        public async Task<ActionResult<PageDetailDto>> GetById(Guid id)
        {
            var page = await _pageUseCase.GetByIdAsync(id);
            if (page == null) return NotFound();
            return Ok(page);
        }

        /// <summary>Create a new page.</summary>
        [HttpPost]
        [RequirePermission("pages.create")]
        public async Task<ActionResult<PageDto>> Create([FromBody] CreatePageRequest request)
        {
            try
            {
                var page = await _pageUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = page.Id }, page);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a page.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("pages.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePageRequest request)
        {
            try
            {
                await _pageUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Delete a page.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("pages.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _pageUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
