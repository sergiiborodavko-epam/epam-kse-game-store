namespace EpamKse.GameStore.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

using Services.Services.Game;
using Domain.Exceptions.Game;
using Domain.Exceptions.Genre;
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
        } catch (GameNotFoundException ex) {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateGame(GameDto gameDto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        try {
            var createdGame = await gameService.CreateGameAsync(gameDto);
            return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Id }, createdGame);
        } catch (GameAlreadyExistsException ex) {
            return Conflict(ex.Message);
        } catch (GenreNamesNotFoundException ex) {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateGame(int id, GameDto gameDto) {
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
        } catch (GenreNamesNotFoundException ex) {
            return NotFound(ex.Message);
        }
    }

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
