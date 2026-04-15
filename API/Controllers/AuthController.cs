using Asp.Versioning;
using Application.DTOs.Auth;
using Application.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [EnableRateLimiting("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthUseCase _authUseCase;

        public AuthController(AuthUseCase authUseCase)
        {
            _authUseCase = authUseCase;
        }

        /// <summary>Register a new user account (assigned Viewer role by default).</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var userId = await _authUseCase.RegisterAsync(request);
                return CreatedAtAction(null, null, new { id = userId });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        /// <summary>Login with email and password. Returns JWT access token and refresh token.</summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var response = await _authUseCase.LoginAsync(request, ip);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        /// <summary>Rotate refresh token. Returns new access token and refresh token.</summary>
        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponse>> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var response = await _authUseCase.RefreshTokenAsync(request.RefreshToken, ip);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        /// <summary>Revoke the provided refresh token (logout current session).</summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _authUseCase.LogoutAsync(request.RefreshToken, ip);
            return NoContent();
        }

        /// <summary>Revoke all refresh tokens for the current user (logout all sessions).</summary>
        [HttpPost("logout-all")]
        [Authorize]
        public async Task<IActionResult> LogoutAll()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _authUseCase.LogoutAllAsync(userId, ip);
            return NoContent();
        }
    }
}
