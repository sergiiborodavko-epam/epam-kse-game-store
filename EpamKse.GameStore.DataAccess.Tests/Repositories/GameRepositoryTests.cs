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
        // Act
        var games = await _repository.GetAllAsync();

        // Assert
        var gamesList = Assert.IsAssignableFrom<IEnumerable<Game>>(games);
        Assert.Equal(2, gamesList.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsGame_WhenGameExists() {
        // Arrange
        const int gameId = 1;

        // Act
        var result = await _repository.GetByIdAsync(gameId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameId, result.Id);
        Assert.Equal("Test Game 1", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenGameDoesNotExist() {
        // Arrange
        const int nonExistentId = 99;

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesAndReturnsGame() {
        // Arrange
        var game = new Game { 
            Title = "New Game", 
            Description = "New Description", 
            Price = 39.99m, 
            ReleaseDate = new DateTime(2023, 3, 1) 
        };

        // Act
        var result = await _repository.CreateAsync(game);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("New Game", result.Title);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesAndReturnsGame() {
        // Arrange
        const int gameId = 1;
        var game = await DbContext.Games.FindAsync(gameId);
        Assert.NotNull(game);
        
        // Update the game
        game.Title = "Updated Game";
        game.Price = 49.99m;

        // Act
        var result = await _repository.UpdateAsync(game);

        // Assert
        Assert.Equal(gameId, result.Id);
        Assert.Equal("Updated Game", result.Title);
        Assert.Equal(49.99m, result.Price);
    }

    [Fact]
    public async Task DeleteAsync_RemovesGameFromDatabase() {
        // Arrange
        const int gameId = 1;
        var game = await DbContext.Games.FindAsync(gameId);
        Assert.NotNull(game);

        // Act
        await _repository.DeleteAsync(game);

        // Assert
        var dbGame = await DbContext.Games.FindAsync(gameId);
        Assert.Null(dbGame);
    }
}
