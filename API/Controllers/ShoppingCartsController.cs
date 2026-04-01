using API.Filters;
using Application.DTOs.Ecommerce;
using Application.UseCases.Ecommerce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/shopping-carts")]
    [Authorize]
    public class ShoppingCartsController : ControllerBase
    {
        private readonly ShoppingCartUseCase _shoppingCartUseCase;

        public ShoppingCartsController(ShoppingCartUseCase shoppingCartUseCase)
        {
            _shoppingCartUseCase = shoppingCartUseCase;
        }

        /// <summary>Get or create an active shopping cart.</summary>
        [HttpGet]
        [RequirePermission("shopping-carts.read")]
        public async Task<ActionResult<ShoppingCartDetailDto>> GetOrCreate(
            [FromQuery] Guid? customerId,
            [FromQuery] Guid? guestId)
        {
            var cart = await _shoppingCartUseCase.GetOrCreateCartAsync(customerId, guestId);
            return Ok(cart);
        }

        /// <summary>Get a shopping cart by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("shopping-carts.read")]
        public async Task<ActionResult<ShoppingCartDetailDto>> GetById(Guid id)
        {
            var cart = await _shoppingCartUseCase.GetByIdAsync(id);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        /// <summary>Add an item to a shopping cart.</summary>
        [HttpPost("{id:guid}/items")]
        [RequirePermission("shopping-carts.manage")]
        public async Task<ActionResult<ShoppingCartDetailDto>> AddItem(Guid id, [FromBody] AddCartItemRequest request)
        {
            try
            {
                var cart = await _shoppingCartUseCase.AddItemAsync(id, request);
                return Ok(cart);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Update a cart item quantity.</summary>
        [HttpPut("{id:guid}/items/{itemId:guid}")]
        [RequirePermission("shopping-carts.manage")]
        public async Task<ActionResult<ShoppingCartDetailDto>> UpdateItem(Guid id, Guid itemId, [FromBody] UpdateCartItemRequest request)
        {
            try
            {
                var cart = await _shoppingCartUseCase.UpdateItemAsync(id, itemId, request);
                return Ok(cart);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Remove an item from a shopping cart.</summary>
        [HttpDelete("{id:guid}/items/{itemId:guid}")]
        [RequirePermission("shopping-carts.manage")]
        public async Task<ActionResult<ShoppingCartDetailDto>> RemoveItem(Guid id, Guid itemId)
        {
            try
            {
                var cart = await _shoppingCartUseCase.RemoveItemAsync(id, itemId);
                return Ok(cart);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Merge a guest cart into a customer cart.</summary>
        [HttpPost("merge")]
        [RequirePermission("shopping-carts.manage")]
        public async Task<IActionResult> MergeGuestCart([FromQuery] Guid customerId, [FromBody] MergeCartRequest request)
        {
            try
            {
                await _shoppingCartUseCase.MergeGuestCartAsync(customerId, request.GuestId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Deactivate a shopping cart.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("shopping-carts.manage")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _shoppingCartUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }
    }
}
