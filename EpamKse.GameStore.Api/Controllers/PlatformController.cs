using EpamKse.GameStore.Domain.DTO.Platform;
using EpamKse.GameStore.Services.Services.Platform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.Api.Controllers;

[ApiController]
[Route("platforms")]
[Authorize(Policy = "UserPolicy")]
public class PlatformController : ControllerBase
{
    private readonly IPlatformService _platformService;

    public PlatformController(IPlatformService platformService)
    {
        _platformService = platformService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPlatforms()
    {
        var platforms = await _platformService.GetPlatforms();
        return Ok(platforms);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPlatformById(int id)
    {
        var platform = await _platformService.GetPlatformById(id);
        return Ok(platform);
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetPlatformByName(string name)
    {
        var platform = await _platformService.GetPlatformByName(name);
        return Ok(platform);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreatePlatform(CreatePlatformDto dto)
    {
        var platform = await _platformService.CreatePlatform(dto);
        return Created($"/platforms/{platform.Id}", platform);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdatePlatform(int id, UpdatePlatformDto dto)
    {
        var platform = await _platformService.UpdatePlatform(id, dto);
        return Ok(platform);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePlatform(int id)
    {
        var platform = await _platformService.DeletePlatform(id);
        return Ok(platform);
    }
}