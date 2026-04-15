using Asp.Versioning;
using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Products;
using Application.UseCases.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ProductUseCase _productUseCase;

        public ProductsController(ProductUseCase productUseCase)
        {
            _productUseCase = productUseCase;
        }

        /// <summary>Get all products (paginated).</summary>
        [HttpGet]
        [RequirePermission("products.read")]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _productUseCase.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>Get a product by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("products.read")]
        public async Task<ActionResult<ProductDto>> GetById(Guid id)
        {
            var product = await _productUseCase.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        /// <summary>Create a new product.</summary>
        [HttpPost]
        [RequirePermission("products.create")]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductRequest request)
        {
            try
            {
                var product = await _productUseCase.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
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

        /// <summary>Update a product.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("products.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
        {
            try
            {
                await _productUseCase.UpdateAsync(id, request);
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

        /// <summary>Delete a product.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("products.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _productUseCase.DeleteAsync(id);
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
