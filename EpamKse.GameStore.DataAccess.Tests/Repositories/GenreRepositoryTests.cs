namespace EpamKse.GameStore.DataAccess.Tests.Repositories;

using Xunit;

using Domain.Entities;
using EpamKse.GameStore.DataAccess.Repositories.Genre;

public class GenreRepositoryTests : BaseRepositoryTests {
    private readonly GenreRepository _repository;

    public GenreRepositoryTests() {
        _repository = new GenreRepository(DbContext);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllGenres() {
        var genres = await _repository.GetAllAsync();
        var genresList = Assert.IsType<IEnumerable<Genre>>(genres, exactMatch: false);
        Assert.Equal(2, genresList.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsGenre_WhenGenreExists() {
        const int genreId = 1;
        var result = await _repository.GetByIdAsync(genreId);
        Assert.NotNull(result);
        Assert.Equal(genreId, result.Id);
        Assert.Equal("Strategy", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenGenreDoesNotExist() {
        const int nonExistentId = 99;
        var result = await _repository.GetByIdAsync(nonExistentId);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_ReturnsGenre_WhenGenreExists() {
        const string name = "Strategy";
        var result = await _repository.GetByNameAsync(name);
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
    }

    [Fact]
    public async Task GetByNameAsync_ReturnsNull_WhenGenreDoesNotExist() {
        const string nonExistentName = "Non Existent Genre";
        var result = await _repository.GetByNameAsync(nonExistentName);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesAndReturnsGenre() {
        var genre = new Genre {
            Name = "Action"
        };

        var result = await _repository.CreateAsync(genre);

        Assert.NotEqual(0, result.Id);
        Assert.Equal("Action", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesAndReturnsGenre() {
        const int genreId = 1;
        var genre = await DbContext.Genres.FindAsync(genreId);
        Assert.NotNull(genre);
        
        genre.Name = "Updated Strategy";

        var result = await _repository.UpdateAsync(genre);

        Assert.Equal(genreId, result.Id);
        Assert.Equal("Updated Strategy", result.Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesGenreFromDatabase() {
        const int genreId = 1;
        var genre = await DbContext.Genres.FindAsync(genreId);
        Assert.NotNull(genre);

        await _repository.DeleteAsync(genre);

        var dbGenre = await DbContext.Genres.FindAsync(genreId);
        Assert.Null(dbGenre);
    }

    [Fact]
    public async Task GetByNamesAsync_ReturnsGenres_WhenGenresExist() {
        var names = new List<string> { "Strategy", "Action" };
        var result = await _repository.GetByNamesAsync(names);
        var genresList = Assert.IsType<List<Genre>>(result, exactMatch: false);
        Assert.Equal(2, genresList.Count);
        Assert.Contains(genresList, g => g.Name == "Strategy");
        Assert.Contains(genresList, g => g.Name == "Action");
    }
}
