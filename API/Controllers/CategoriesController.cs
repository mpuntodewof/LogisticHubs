using Asp.Versioning;
using API.Filters;
using Domain.Constants;
using Application.DTOs.Categories;
using Application.DTOs.Common;
using Application.UseCases.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/categories")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryUseCase _categoryUseCase;

        public CategoriesController(CategoryUseCase categoryUseCase)
        {
            _categoryUseCase = categoryUseCase;
        }

        /// <summary>Get all categories (paginated).</summary>
        [HttpGet]
        [RequirePermission(Permissions.Categories.Read)]
        public async Task<ActionResult<PagedResult<CategoryDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _categoryUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get category tree.</summary>
        [HttpGet("tree")]
        [RequirePermission(Permissions.Categories.Read)]
        public async Task<ActionResult<IEnumerable<CategoryTreeDto>>> GetTree()
        {
            var result = await _categoryUseCase.GetTreeAsync();
            return Ok(result);
        }

        /// <summary>Get a category by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission(Permissions.Categories.Read)]
        public async Task<ActionResult<CategoryDto>> GetById(Guid id)
        {
            var category = await _categoryUseCase.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        /// <summary>Create a new category.</summary>
        [HttpPost]
        [RequirePermission(Permissions.Categories.Create)]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var category = await _categoryUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
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

        /// <summary>Update a category.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission(Permissions.Categories.Update)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
        {
            try
            {
                await _categoryUseCase.UpdateAsync(id, request);
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

        /// <summary>Delete a category.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission(Permissions.Categories.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _categoryUseCase.DeleteAsync(id);
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
