using API.Filters;
using Application.DTOs.Settings;
using Application.UseCases.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/system-settings")]
    [Authorize]
    public class SystemSettingsController : ControllerBase
    {
        private readonly SystemSettingUseCase _systemSettingUseCase;

        public SystemSettingsController(SystemSettingUseCase systemSettingUseCase)
        {
            _systemSettingUseCase = systemSettingUseCase;
        }

        /// <summary>Get all system settings, optionally filtered by group.</summary>
        [HttpGet]
        [RequirePermission("system-settings.read")]
        public async Task<ActionResult<List<SystemSettingDto>>> GetAll([FromQuery] string? group = null)
        {
            var result = await _systemSettingUseCase.GetAllAsync(group);
            return Ok(result);
        }

        /// <summary>Get a system setting by key.</summary>
        [HttpGet("by-key/{key}")]
        [RequirePermission("system-settings.read")]
        public async Task<ActionResult<SystemSettingDto>> GetByKey(string key)
        {
            var result = await _systemSettingUseCase.GetByKeyAsync(key);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>Update a system setting.</summary>
        [HttpPut("{id:guid}")]
        [RequirePermission("system-settings.update")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSystemSettingRequest request)
        {
            try
            {
                await _systemSettingUseCase.UpdateAsync(id, request);
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
    }
}
