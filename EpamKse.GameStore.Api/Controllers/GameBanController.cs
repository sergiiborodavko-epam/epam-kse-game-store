namespace EpamKse.GameStore.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Domain.DTO.GameBan;
using Domain.Enums;
using Services.Services.GameBan;

[ApiController]
[Route("api/game-bans")]
[Authorize(Roles = "Admin")]
public class GameBanController(IGameBanService banService) : ControllerBase {
    [HttpGet]
    public async Task<IActionResult> GetAllBans() {
        var bans = await banService.GetAllBansAsync();
        return Ok(bans);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBanById(int id) {
        var ban = await banService.GetBanByIdAsync(id);
        return Ok(ban);
    }

    [HttpGet("country/{country}")]
    public async Task<IActionResult> GetBansByCountry(string country) {
        var bans = await banService.GetBansByCountryAsync(country);
        return Ok(bans);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBan(CreateGameBanDto dto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        var ban = await banService.CreateBanAsync(dto);
        return CreatedAtAction(nameof(GetBanById), new { id = ban.Id }, ban);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBan(int id) {
        await banService.DeleteBanAsync(id);
        return NoContent();
    }
}
