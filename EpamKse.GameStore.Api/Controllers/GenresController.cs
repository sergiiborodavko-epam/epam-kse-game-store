namespace EpamKse.GameStore.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

using Services.Services.Genre;
using Domain.Exceptions.Genre;
using Domain.DTO.Genre;

[ApiController]
[Route("api/genres")]
public class GenresController(IGenreService genreService) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> GetAllGenres() {
        var genres = await genreService.GetAllGenresAsync();
        return Ok(genres);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetGenreById(int id) {
        try {
            var genre = await genreService.GetGenreByIdAsync(id);
            return Ok(genre);
        } catch (GenreIdNotFoundException ex) {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateGenre(GenreDto genreDto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        
        try {
            var createdGenre = await genreService.CreateGenreAsync(genreDto);
            return CreatedAtAction(nameof(GetGenreById), new { id = createdGenre.Id }, createdGenre);
        } catch (GenreAlreadyExistsException ex) {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateGenre(int id, GenreDto genreDto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        
        try {
            var updatedGenre = await genreService.UpdateGenreAsync(id, genreDto);
            return Ok(updatedGenre);
        } catch (GenreIdNotFoundException ex) {
            return NotFound(ex.Message);
        } catch (GenreAlreadyExistsException ex) {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGenre(int id) {
        try {
            await genreService.DeleteGenreAsync(id);
            return NoContent();
        } catch (GenreIdNotFoundException ex) {
            return NotFound(ex.Message);
        }
    }
}
