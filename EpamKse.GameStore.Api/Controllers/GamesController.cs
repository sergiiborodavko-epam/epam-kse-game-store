namespace EpamKse.GameStore.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

using Services.Services.Game;
using Domain.DTO.Game;

[ApiController]
[Route("api/games")]
public class GamesController(IGameService gameService) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> GetAllGames() {
        var games = await gameService.GetAllGamesAsync();
        return Ok(games);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetGameById(int id) {
        var game = await gameService.GetGameByIdAsync(id);
        return Ok(game);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGame(GameDto gameDto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        var createdGame = await gameService.CreateGameAsync(gameDto);
        return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Id }, createdGame);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateGame(int id, GameDto gameDto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        var updatedGame = await gameService.UpdateGameAsync(id, gameDto);
        return Ok(updatedGame);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGame(int id) {
        await gameService.DeleteGameAsync(id);
        return NoContent();
    }
}
