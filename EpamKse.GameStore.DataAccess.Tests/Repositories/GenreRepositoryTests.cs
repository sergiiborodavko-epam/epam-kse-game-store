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
    public async Task GetAllAsync_ReturnsAllGenresWithRelations() {
        var genres = await _repository.GetAllAsync();
        var genresList = genres.ToList();
        
        Assert.Equal(3, genresList.Count);
        Assert.Contains(genresList, g => g is { Name: "Strategy", ParentGenreId: null });
        Assert.Contains(genresList, g => g is { Name: "RTS", ParentGenreId: 1 });
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsGenreWithRelations() {
        var result = await _repository.GetByIdAsync(2);
        
        Assert.NotNull(result);
        Assert.Equal("RTS", result.Name);
        Assert.Equal(1, result.ParentGenreId);
        Assert.NotNull(result.ParentGenre);
        Assert.Equal("Strategy", result.ParentGenre.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull() {
        var result = await _repository.GetByIdAsync(999);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_ExistingName_ReturnsGenre() {
        var result = await _repository.GetByNameAsync("Strategy");
        
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Null(result.ParentGenreId);
    }

    [Fact]
    public async Task GetByNameAsync_NonExistentName_ReturnsNull() {
        var result = await _repository.GetByNameAsync("NonExistent");
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNamesAsync_MultipleNames_ReturnsMatchingGenres() {
        var names = new List<string> { "Strategy", "RTS", "NonExistent" };
        var result = await _repository.GetByNamesAsync(names);
        
        Assert.Equal(2, result.Count);
        Assert.Contains(result, g => g.Name == "Strategy");
        Assert.Contains(result, g => g.Name == "RTS");
    }

    [Fact]
    public async Task GetMainGenresAsync_ReturnsOnlyParentGenres() {
        var result = await _repository.GetMainGenresAsync();
        
        Assert.Equal(2, result.Count);
        Assert.All(result, g => Assert.Null(g.ParentGenreId));
        Assert.Contains(result, g => g.Name == "Strategy");
        Assert.Contains(result, g => g.Name == "Action");
    }

    [Fact]
    public async Task GetSubGenresByParentNameAsync_ValidParent_ReturnsSubGenres() {
        var result = await _repository.GetSubGenresByParentNameAsync("Strategy");
        
        Assert.Single(result);
        Assert.Equal("RTS", result.First().Name);
        Assert.Equal("Strategy", result.First().ParentGenre?.Name);
    }

    [Fact]
    public async Task GetSubGenresByParentNameAsync_NoSubGenres_ReturnsEmpty() {
        var result = await _repository.GetSubGenresByParentNameAsync("Action");
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_MainGenre_CreatesSuccessfully() {
        var genre = new Genre { Name = "RPG", ParentGenreId = null };
        
        var result = await _repository.CreateAsync(genre);
        
        Assert.NotEqual(0, result.Id);
        Assert.Equal("RPG", result.Name);
        Assert.Null(result.ParentGenreId);
    }

    [Fact]
    public async Task CreateAsync_SubGenre_CreatesSuccessfully() {
        var subGenre = new Genre { Name = "TBS", ParentGenreId = 1 };
        
        var result = await _repository.CreateAsync(subGenre);
        
        Assert.NotEqual(0, result.Id);
        Assert.Equal("TBS", result.Name);
        Assert.Equal(1, result.ParentGenreId);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesGenreSuccessfully() {
        var genre = await DbContext.Genres.FindAsync(1);
        genre!.Name = "Updated Strategy";

        var result = await _repository.UpdateAsync(genre);

        Assert.Equal("Updated Strategy", result.Name);
        var dbGenre = await DbContext.Genres.FindAsync(1);
        Assert.Equal("Updated Strategy", dbGenre?.Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesGenreFromDatabase() {
        var genre = await DbContext.Genres.FindAsync(3);

        await _repository.DeleteAsync(genre!);

        var deletedGenre = await DbContext.Genres.FindAsync(3);
        Assert.Null(deletedGenre);
    }
}
