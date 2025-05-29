using Microsoft.AspNetCore.Authorization;

namespace EpamKse.GameStore.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

using Services.Services.Game;
using Domain.Exceptions;
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
    public async Task<IActionResult> GetGameById(int id) {
        try {
            var game = await gameService.GetGameByIdAsync(id);
            return Ok(game);
        } catch (GameNotFoundException) {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateGame(GameDTO gameDto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        
        try {
            var createdGame = await gameService.CreateGameAsync(gameDto);
            return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Id }, createdGame);
        } catch (GameAlreadyExistsException ex) {
            return Conflict(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateGame(int id, GameDTO gameDto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        try {
            var updatedGame = await gameService.UpdateGameAsync(id, gameDto);
            return Ok(updatedGame);
        } catch (GameNotFoundException) {
            return NotFound();
        } catch (GameAlreadyExistsException ex) {
            return Conflict(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGame(int id) {
        try {
            await gameService.DeleteGameAsync(id);
            return NoContent();
        } catch (GameNotFoundException) {
            return NotFound();
        }
    }
}
