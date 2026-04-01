using API.Filters;
using Application.DTOs.Settings;
using Application.UseCases.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/tenant-settings")]
    [Authorize]
    public class TenantSettingsController : ControllerBase
    {
        private readonly TenantSettingUseCase _tenantSettingUseCase;

        public TenantSettingsController(TenantSettingUseCase tenantSettingUseCase)
        {
            _tenantSettingUseCase = tenantSettingUseCase;
        }

        /// <summary>Get all tenant settings, optionally filtered by group.</summary>
        [HttpGet]
        [RequirePermission("tenant-settings.read")]
        public async Task<ActionResult<List<TenantSettingDto>>> GetAll([FromQuery] string? group = null)
        {
            var result = await _tenantSettingUseCase.GetAllAsync(group);
            return Ok(result);
        }

        /// <summary>Get a tenant setting by key.</summary>
        [HttpGet("by-key/{key}")]
        [RequirePermission("tenant-settings.read")]
        public async Task<ActionResult<TenantSettingDto>> GetByKey(string key)
        {
            var result = await _tenantSettingUseCase.GetByKeyAsync(key);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>Update a tenant setting.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("tenant-settings.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenantSettingRequest request)
        {
            try
            {
                await _tenantSettingUseCase.UpdateAsync(id, request);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Bulk update tenant settings.</summary>
        [HttpPut("bulk")]
        [RequirePermission("tenant-settings.update")]
        public async Task<IActionResult> BulkUpdate([FromBody] BulkUpdateTenantSettingsRequest request)
        {
            await _tenantSettingUseCase.BulkUpdateAsync(request);
            return NoContent();
        }
    }
}
