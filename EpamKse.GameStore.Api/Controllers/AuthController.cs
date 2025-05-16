using EpamKse.GameStore.Api.DTO.Auth;
using EpamKse.GameStore.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.Api.Controllers;


[ApiController]
[Route("auth")]
public class AuthController:ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO request)
    {
        var at =await _authService.Register(request);
        return Ok(new { AccessToken = at });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Register([FromBody] LoginDTO request)
    {
        var at =await _authService.Login(request);
        return Ok(new { AccessToken = at });
    }
}