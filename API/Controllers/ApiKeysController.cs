using API.Filters;
using Application.DTOs.Common;
using Application.DTOs.Settings;
using Application.UseCases.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/api-keys")]
    [Authorize]
    public class ApiKeysController : ControllerBase
    {
        private readonly ApiKeyUseCase _apiKeyUseCase;

        public ApiKeysController(ApiKeyUseCase apiKeyUseCase)
        {
            _apiKeyUseCase = apiKeyUseCase;
        }

        /// <summary>Get all API keys (paginated).</summary>
        [HttpGet]
        [RequirePermission("api-keys.read")]
        public async Task<ActionResult<PagedResult<ApiKeyDto>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] bool? isActive = null)
        {
            var result = await _apiKeyUseCase.GetPagedAsync(request, isActive);
            return Ok(result);
        }

        /// <summary>Get an API key by ID.</summary>
        [HttpGet("{id:guid}")]
        [RequirePermission("api-keys.read")]
        public async Task<ActionResult<ApiKeyDetailDto>> GetById(Guid id)
        {
            var apiKey = await _apiKeyUseCase.GetByIdAsync(id);
            if (apiKey == null) return NotFound();
            return Ok(apiKey);
        }

        /// <summary>Create a new API key. The plain text key is returned only once.</summary>
        [HttpPost]
        [RequirePermission("api-keys.create")]
        public async Task<ActionResult<CreateApiKeyResponse>> Create([FromBody] CreateApiKeyRequest request)
        {
            var result = await _apiKeyUseCase.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Update an API key.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("api-keys.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateApiKeyRequest request)
        {
            try
            {
                await _apiKeyUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Regenerate an API key. The new plain text key is returned only once.</summary>
        [HttpPost("{id:guid}/regenerate")]
        [RequirePermission("api-keys.regenerate")]
        public async Task<ActionResult<CreateApiKeyResponse>> Regenerate(Guid id)
        {
            try
            {
                var result = await _apiKeyUseCase.RegenerateAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Delete an API key.</summary>
        [HttpDelete("{id:guid}")]
        [RequirePermission("api-keys.delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _apiKeyUseCase.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
