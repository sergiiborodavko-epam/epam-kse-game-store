using EpamKse.GameStore.Domain.DTO.License;
using EpamKse.GameStore.Services.Services.License;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EpamKse.GameStore.Api.Controllers;

[ApiController]
[Route("licenses")]
[Authorize(AuthenticationSchemes = "Access")]
public class LicensesController : ControllerBase
{
    private readonly ILicenseService _licenseService;

    public LicensesController(ILicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyLicense([FromBody] VerifyLicenseDto dto)
    {
        _licenseService.VerifyLicense(dto.Key);
        return NoContent();
    }

    [HttpGet("order/{id:int}")]
    public async Task<IActionResult> GetLicensesByOrderId(int id)
    {
        var txt = await _licenseService.GenerateLicenseFileByOrderId(id);
        return File(txt, "text/plain", "license.txt");
    }

    [HttpGet("game/{id:int}")]
    public async Task<IActionResult> GetLicensesByGameId(int id)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")!.Value;
        var userId = int.Parse(userIdClaim);
        
        var txt = await _licenseService.GenerateLicenseFileByGameId(userId, id);
        return File(txt, "text/plain", "license.txt");   
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateLicense(CreateLicenseDto dto)
    {
        var license = await _licenseService.CreateLicense(dto);
        return Ok(new
        {
            licenseKey = license.Key
        });   
    }
}