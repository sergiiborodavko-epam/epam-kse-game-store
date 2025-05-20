namespace EpamKse.GameStore.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

using Services.Services.Game;
using Domain.DTO;

[ApiController]
[Route("api/games")]
public class GamesController(IGameService gameService) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> GetAllGames() {
        var games = await gameService.GetAllGamesAsync();
        return Ok(games);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GameDTO>> GetGameById(int id) {
        var game = await gameService.GetGameByIdAsync(id);
        if (game == null) {
            return NotFound();
        }
        
        return Ok(game);
    }

    [HttpPost]
    public async Task<ActionResult<GameDTO>> CreateGame(GameDTO gameDto) {
        var createdGame = await gameService.CreateGameAsync(gameDto);
        return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Id }, createdGame);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<GameDTO>> UpdateGame(int id, GameDTO gameDto) {
        var updatedGame = await gameService.UpdateGameAsync(id, gameDto);
        if (updatedGame == null) {
            return NotFound();
        }
        
        return Ok(updatedGame);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGame(int id) {
        var result = await gameService.DeleteGameAsync(id);
        if (!result) {
            return NotFound();
        }
        
        return NoContent();
    }
}
