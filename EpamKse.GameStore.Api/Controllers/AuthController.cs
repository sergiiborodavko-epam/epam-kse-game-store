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
            var accessToken = await _authService.Register(request);
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
            var accessToken = await _authService.Login(request);
            return Ok(new { AccessToken = accessToken });
        }
        catch (InvalidCredentialsException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}