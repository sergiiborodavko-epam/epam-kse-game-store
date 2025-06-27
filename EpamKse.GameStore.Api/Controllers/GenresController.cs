using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace EpamKse.GameStore.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

using Services.Services.Genre;
using Domain.DTO.Genre;

[ApiController]
[Authorize(Policy = "UserPolicy")]
[Route("api/genres")]
public class GenresController(IGenreService genreService, IMapper _mapper) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> GetAllGenres() {
        var genres = await genreService.GetAllGenresAsync();
        return Ok(_mapper.Map<IEnumerable<GenreViewDto>>(genres));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetGenreById(int id) {
        var genre = await genreService.GetGenreByIdAsync(id);
        return Ok(_mapper.Map<GenreViewDto>(genre));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateGenre(GenreDto genreDto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        
        var createdGenre = await genreService.CreateGenreAsync(genreDto);
        return CreatedAtAction(nameof(GetGenreById), new { id = createdGenre.Id }, _mapper.Map<GenreViewDto>(createdGenre));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateGenre(int id, GenreDto genreDto) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }
        
        var updatedGenre = await genreService.UpdateGenreAsync(id, genreDto);
        return Ok(_mapper.Map<GenreViewDto>(updatedGenre));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGenre(int id) {
        await genreService.DeleteGenreAsync(id);
        return NoContent();
    }
}
