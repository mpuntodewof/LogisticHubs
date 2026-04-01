using Application.DTOs.Ecommerce;
using Application.Interfaces;
using Application.UseCases.Ecommerce;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/storefront/cart")]
    public class StorefrontCartController : ControllerBase
    {
        private readonly ShoppingCartUseCase _cartUseCase;
        private readonly ICurrentUserService _currentUserService;

        public StorefrontCartController(
            ShoppingCartUseCase cartUseCase,
            ICurrentUserService currentUserService)
        {
            _cartUseCase = cartUseCase;
            _currentUserService = currentUserService;
        }

        /// <summary>Get or create a shopping cart.</summary>
        [HttpGet]
        public async Task<ActionResult<ShoppingCartDetailDto>> GetCart(
            [FromQuery] Guid? customerId = null,
            [FromQuery] Guid? guestId = null)
        {
            var resolvedCustomerId = ResolveCustomerId(customerId);
            var cart = await _cartUseCase.GetOrCreateCartAsync(resolvedCustomerId, guestId);
            return Ok(cart);
        }

        /// <summary>Add an item to the cart.</summary>
        [HttpPost("items")]
        public async Task<ActionResult<ShoppingCartDetailDto>> AddItem([FromBody] StorefrontAddCartItemRequest request)
        {
            try
            {
                var resolvedCustomerId = ResolveCustomerId(null);
                var cart = await _cartUseCase.GetOrCreateCartAsync(resolvedCustomerId, request.GuestId);
                var result = await _cartUseCase.AddItemAsync(cart.Id, new AddCartItemRequest
                {
                    ProductVariantId = request.ProductVariantId,
                    Quantity = request.Quantity
                });
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>Update a cart item quantity.</summary>
        [HttpPut("items/{itemId:guid}")]
        public async Task<ActionResult<ShoppingCartDetailDto>> UpdateItem(
            Guid itemId,
            [FromBody] UpdateCartItemRequest request,
            [FromQuery] Guid? customerId = null,
            [FromQuery] Guid? guestId = null)
        {
            try
            {
                var resolvedCustomerId = ResolveCustomerId(customerId);
                var cart = await _cartUseCase.GetOrCreateCartAsync(resolvedCustomerId, guestId);
                var result = await _cartUseCase.UpdateItemAsync(cart.Id, itemId, request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>Remove a cart item.</summary>
        [HttpDelete("items/{itemId:guid}")]
        public async Task<ActionResult<ShoppingCartDetailDto>> RemoveItem(
            Guid itemId,
            [FromQuery] Guid? customerId = null,
            [FromQuery] Guid? guestId = null)
        {
            try
            {
                var resolvedCustomerId = ResolveCustomerId(customerId);
                var cart = await _cartUseCase.GetOrCreateCartAsync(resolvedCustomerId, guestId);
                var result = await _cartUseCase.RemoveItemAsync(cart.Id, itemId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>Merge guest cart into authenticated customer cart.</summary>
        [HttpPost("merge")]
        public async Task<IActionResult> MergeCart([FromBody] MergeCartRequest request)
        {
            if (User.Identity?.IsAuthenticated != true)
                return Unauthorized(new { error = "Authentication required to merge carts." });

            var customerId = ResolveCustomerId(null);
            if (!customerId.HasValue)
                return Unauthorized(new { error = "Could not resolve customer identity." });

            try
            {
                await _cartUseCase.MergeGuestCartAsync(customerId.Value, request.GuestId);
                return Ok(new { message = "Cart merged successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private Guid? ResolveCustomerId(Guid? explicitCustomerId)
        {
            if (explicitCustomerId.HasValue)
                return explicitCustomerId;

            if (_currentUserService.IsAuthenticated)
                return _currentUserService.UserId;

            return null;
        }
    }

    public class StorefrontAddCartItemRequest
    {
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public Guid? GuestId { get; set; }
    }
}
