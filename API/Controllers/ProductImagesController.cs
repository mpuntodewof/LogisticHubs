using Asp.Versioning;
using API.Filters;
using Domain.Constants;
using Application.DTOs.Common;
using Application.DTOs.Products;
using Application.UseCases.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/product-images")]
    [Authorize]
    public class ProductImagesController : ControllerBase
    {
        private readonly ProductImageUseCase _productImageUseCase;

        public ProductImagesController(ProductImageUseCase productImageUseCase)
        {
            _productImageUseCase = productImageUseCase;
        }

        /// <summary>Get images for a product.</summary>
        [HttpGet]
        [RequirePermission(Permissions.Products.Read)]
        public async Task<ActionResult<IEnumerable<ProductImageDto>>> GetByProduct([FromQuery] Guid productId)
        {
            try
            {
                var result = await _productImageUseCase.GetByProductIdAsync(productId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Create a new product image.</summary>
        [HttpPost]
        [RequirePermission(Permissions.Products.Update)]
        public async Task<ActionResult<ProductImageDto>> Create([FromBody] CreateProductImageRequest request)
        {
            try
            {
                var image = await _productImageUseCase.CreateAsync(request);
                return Ok(image);
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

        /// <summary>Delete a product image.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission(Permissions.Products.Update)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _productImageUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
