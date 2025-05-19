using EpamKse.GameStore.Api.DTO.Platform;
using EpamKse.GameStore.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.Api.Controllers;

[ApiController]
[Route("platforms")]
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
        return platform == null ? NotFound() : Ok(platform);
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetPlatformByName(string name)
    {
        var platform = await _platformService.GetPlatformByName(name);
        return platform == null ? NotFound() : Ok(platform);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlatform(CreatePlatformDto dto)
    {
        var platform = await _platformService.CreatePlatform(dto);
        return platform == null ? Conflict($"Platform with name '{dto.Name}' already exists") : Created($"/platforms/{platform.Id}", platform);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdatePlatform(int id, UpdatePlatformDto dto)
    {
        var platform = await _platformService.UpdatePlatform(id, dto);
        return platform == null ? NotFound() : Ok(platform);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePlatform(int id)
    {
        var platform = await _platformService.DeletePlatform(id);
        return platform == null ? NotFound() : Ok(platform);
    }
}