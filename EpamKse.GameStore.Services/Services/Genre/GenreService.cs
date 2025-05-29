namespace EpamKse.GameStore.Services.Services.Genre;

using Domain.DTO.Genre;
using Domain.Entities;
using Domain.Exceptions.Genre;
using DataAccess.Repositories.Genre;

public class GenreService(IGenreRepository genreRepository) : IGenreService {
    public async Task<IEnumerable<Genre>> GetAllGenresAsync() {
        return await genreRepository.GetAllAsync();
    }

    public async Task<Genre> GetGenreByIdAsync(int id) {
        var genre = await genreRepository.GetByIdAsync(id);
        return genre ?? throw new GenreIdNotFoundException(id);
    }

    public async Task<Genre> CreateGenreAsync(GenreDto genreDto) {
        var existingGenre = await genreRepository.GetByNameAsync(genreDto.Name);
        if (existingGenre != null) {
            throw new GenreAlreadyExistsException(genreDto.Name);
        }

        var genre = new Genre {
            Name = genreDto.Name
        };
        
        return await genreRepository.CreateAsync(genre);
    }

    public async Task<Genre> UpdateGenreAsync(int id, GenreDto genreDto) {
        var existingGenre = await genreRepository.GetByIdAsync(id);
        if (existingGenre == null) {
            throw new GenreIdNotFoundException(id);
        }

        var genreWithSameName = await genreRepository.GetByNameAsync(genreDto.Name);
        if (genreWithSameName != null && genreWithSameName.Id != id) {
            throw new GenreAlreadyExistsException(genreDto.Name);
        }
        
        existingGenre.Name = genreDto.Name;
        return await genreRepository.UpdateAsync(existingGenre);
    }

    public async Task DeleteGenreAsync(int id) {
        var existingGenre = await genreRepository.GetByIdAsync(id);
        if (existingGenre == null) {
            throw new GenreIdNotFoundException(id);
        }

        await genreRepository.DeleteAsync(existingGenre);
    }
}
