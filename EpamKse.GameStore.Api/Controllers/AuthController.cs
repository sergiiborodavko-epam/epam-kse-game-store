using System.Security.Claims;
using EpamKse.GameStore.Api.DTO.Auth;
using EpamKse.GameStore.Api.Exceptions.Auth;
using EpamKse.GameStore.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO request)
    {
        try
        {
            var (accessToken, refreshToken) = await _authService.Register(request);
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
            return Ok(new { AccessToken = accessToken });
        }
        catch (UserAlreadyExistsException ex)
        {
            return Conflict(new { ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO request)
    {
        try
        {
            var (accessToken, refreshToken) = await _authService.Login(request);
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(new { AccessToken = accessToken });
        }
        catch (InvalidCredentialsException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpGet("refresh")]
    public async Task<IActionResult> Refresh()
    {
        try
        {
            if (HttpContext.Items.TryGetValue("RefreshTokenClaims", out var obj) && obj is ClaimsPrincipal principal)
            {
                var email = principal.FindFirst(ClaimTypes.Email)?.Value;
                var role = principal.FindFirst(ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
                    return Unauthorized("Invalid token claims.");

                var (accessToken, refreshToken) = await _authService.Refresh(email);

                Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                return Ok(new { AccessToken = accessToken });
            }

            return Unauthorized("Missing or invalid refresh token.");
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error.");
        }
    }
}