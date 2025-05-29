namespace EpamKse.GameStore.Services.Services.Genre;

using Domain.DTO.Genre;
using Domain.Entities;

public interface IGenreService {
    Task<IEnumerable<Genre>> GetAllGenresAsync();
    Task<Genre> GetGenreByIdAsync(int id);
    Task<Genre> CreateGenreAsync(GenreDto genreDto);
    Task<Genre> UpdateGenreAsync(int id, GenreDto genreDto);
    Task DeleteGenreAsync(int id);
}
