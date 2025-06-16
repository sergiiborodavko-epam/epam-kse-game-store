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

    [HttpGet]
    public async Task<IActionResult> GetLicenses([FromQuery] GetLicenseQueryDto query)
    {
        if (query.OrderId != null)
        {
            var txt = await _licenseService.GenerateLicenseFileByOrderId(query.OrderId.Value);
            return File(txt, "text/plain", "license.txt");
        }

        if (query.GameId != null)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")!.Value;
            var userId = int.Parse(userIdClaim);
            
            var txt = await _licenseService.GenerateLicenseFileByGameId(userId, query.GameId.Value);
            return File(txt, "text/plain", "license.txt");
        }
        
        return BadRequest("Invalid query");
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