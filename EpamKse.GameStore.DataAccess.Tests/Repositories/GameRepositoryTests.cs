namespace EpamKse.GameStore.DataAccess.Tests.Repositories;

using Xunit;

using Domain.Entities;
using EpamKse.GameStore.DataAccess.Repositories.Game;

public class GameRepositoryTests : BaseRepositoryTests {
    private readonly GameRepository _repository;

    public GameRepositoryTests() {
        _repository = new GameRepository(DbContext);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllGames() {
        var games = await _repository.GetAllAsync();

        var gamesList = Assert.IsAssignableFrom<IEnumerable<Game>>(games);
        Assert.Equal(2, gamesList.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsGame() {
        var result = await _repository.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Game 1", result.Title);
        Assert.Equal(2, result.GenreIds.Count);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull() {
        var result = await _repository.GetByIdAsync(99);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByTitleAsync_ExistingTitle_ReturnsGame() {
        var result = await _repository.GetByTitleAsync("Test Game 1");

        Assert.NotNull(result);
        Assert.Equal("Test Game 1", result.Title);
        Assert.Contains(1, result.GenreIds);
        Assert.Contains(2, result.GenreIds);
    }

    [Fact]
    public async Task GetByTitleAsync_NonExistentTitle_ReturnsNull() {
        var result = await _repository.GetByTitleAsync("Non Existent Game");
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WithMultipleGenres_Success() {
        var game = new Game {
            Title = "New Game",
            Description = "New Description",
            Price = 39.99m,
            Stock = 12,
            ReleaseDate = new DateTime(2023, 3, 1),
            GenreIds = [1, 3] // Strategy and Action
        };

        var result = await _repository.CreateAsync(game);

        Assert.NotEqual(0, result.Id);
        Assert.Equal("New Game", result.Title);
        Assert.Equal(2, result.GenreIds.Count);
        Assert.Contains(1, result.GenreIds);
        Assert.Contains(3, result.GenreIds);
    }

    [Fact]
    public async Task UpdateAsync_ChangeGenres_Success() {
        var game = await DbContext.Games.FindAsync(1);
        Assert.NotNull(game);
        
        game.Title = "Updated Game";
        game.Price = 49.99m;
        game.GenreIds = [3]; // Change from Strategy+RTS to Action only

        var result = await _repository.UpdateAsync(game);

        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Game", result.Title);
        Assert.Equal(49.99m, result.Price);
        Assert.Single(result.GenreIds);
        Assert.Contains(3, result.GenreIds);
    }

    [Fact]
    public async Task DeleteAsync_RemovesGameFromDatabase() {
        var game = await DbContext.Games.FindAsync(1);
        Assert.NotNull(game);

        await _repository.DeleteAsync(game);

        var dbGame = await DbContext.Games.FindAsync(1);
        Assert.Null(dbGame);
    }
}
