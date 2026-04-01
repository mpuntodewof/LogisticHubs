using API.Filters;
using Application.DTOs.Storefront;
using Application.UseCases.Storefront;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/admin/storefront-config")]
    [Authorize]
    public class StorefrontConfigController : ControllerBase
    {
        private readonly StorefrontConfigUseCase _storefrontConfigUseCase;

        public StorefrontConfigController(StorefrontConfigUseCase storefrontConfigUseCase)
        {
            _storefrontConfigUseCase = storefrontConfigUseCase;
        }

        /// <summary>Get storefront configuration.</summary>
        [HttpGet]
        [RequirePermission("storefront-config.read")]
        public async Task<ActionResult<StorefrontConfigDto>> Get()
        {
            var result = await _storefrontConfigUseCase.GetAsync();
            return Ok(result);
        }

        /// <summary>Update storefront configuration.</summary>
        [HttpPut]
        [RequirePermission("storefront-config.update")]
        public async Task<IActionResult> Update([FromBody] UpdateStorefrontConfigRequest request)
        {
            try
            {
                await _storefrontConfigUseCase.UpdateAsync(request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
