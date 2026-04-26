using Asp.Versioning;
using API.Filters;
using Domain.Constants;
using Application.DTOs.Brands;
using Application.DTOs.Common;
using Application.UseCases.Brands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/brands")]
    [Authorize]
    public class BrandsController : ControllerBase
    {
        private readonly BrandUseCase _brandUseCase;

        public BrandsController(BrandUseCase brandUseCase)
        {
            _brandUseCase = brandUseCase;
        }

        /// <summary>Get all brands (paginated).</summary>
        [HttpGet]
        [RequirePermission(Permissions.Brands.Read)]
        public async Task<ActionResult<PagedResult<BrandDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _brandUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a brand by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission(Permissions.Brands.Read)]
        public async Task<ActionResult<BrandDto>> GetById(Guid id)
        {
            var brand = await _brandUseCase.GetByIdAsync(id);
            if (brand == null) return NotFound();
            return Ok(brand);
        }

        /// <summary>Create a new brand.</summary>
        [HttpPost]
        [RequirePermission(Permissions.Brands.Create)]
        public async Task<ActionResult<BrandDto>> Create([FromBody] CreateBrandRequest request)
        {
            try
            {
                var brand = await _brandUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = brand.Id }, brand);
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

        /// <summary>Update a brand.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission(Permissions.Brands.Update)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBrandRequest request)
        {
            try
            {
                await _brandUseCase.UpdateAsync(id, request);
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

        /// <summary>Delete a brand.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission(Permissions.Brands.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _brandUseCase.DeleteAsync(id);
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
