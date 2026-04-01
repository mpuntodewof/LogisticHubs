using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Products;
using Application.UseCases.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/product-variants")]
    [Authorize]
    public class ProductVariantsController : ControllerBase
    {
        private readonly ProductVariantUseCase _productVariantUseCase;

        public ProductVariantsController(ProductVariantUseCase productVariantUseCase)
        {
            _productVariantUseCase = productVariantUseCase;
        }

        /// <summary>Get all product variants (paginated, optionally filtered by product).</summary>
        [HttpGet]
        [RequirePermission("products.read")]
        public async Task<ActionResult<PagedResult<ProductVariantDto>>> GetAll([FromQuery] PagedRequest request, [FromQuery] Guid? productId)
        {
            var result = await _productVariantUseCase.GetPagedAsync(request, productId);
            return Ok(result);
        }

        /// <summary>Get a product variant by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("products.read")]
        public async Task<ActionResult<ProductVariantDto>> GetById(Guid id)
        {
            var variant = await _productVariantUseCase.GetByIdAsync(id);
            if (variant == null) return NotFound();
            return Ok(variant);
        }

        /// <summary>Create a new product variant.</summary>
        [HttpPost]
        [RequirePermission("products.create")]
        public async Task<ActionResult<ProductVariantDto>> Create([FromBody] CreateProductVariantRequest request)
        {
            try
            {
                var variant = await _productVariantUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = variant.Id }, variant);
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

        /// <summary>Update a product variant.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("products.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductVariantRequest request)
        {
            try
            {
                await _productVariantUseCase.UpdateAsync(id, request);
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

        /// <summary>Delete a product variant.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("products.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _productVariantUseCase.DeleteAsync(id);
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
