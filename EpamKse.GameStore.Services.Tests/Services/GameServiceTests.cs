namespace EpamKse.GameStore.Services.Tests.Services;

using Xunit;
using Moq;

using Domain.DTO;
using Domain.Entities;
using Domain.Exceptions;
using DataAccess.Repositories.Game;
using EpamKse.GameStore.Services.Services.Game;

public class GameServiceTests {
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly GameService _gameService;

    public GameServiceTests() {
        _mockGameRepository = new Mock<IGameRepository>();
        _gameService = new GameService(_mockGameRepository.Object);
    }

    [Fact]
    public async Task GetAllGamesAsync_ReturnsAllGames() {
        // Arrange
        var games = new List<Game> {
            new() { Id = 1, Title = "Test Game 1", Description = "Test Description 1", Price = 19.99m },
            new() { Id = 2, Title = "Test Game 2", Description = "Test Description 2", Price = 29.99m }
        };
        _mockGameRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(games);

        // Act
        var result = await _gameService.GetAllGamesAsync();

        // Assert
        var returnedGames = Assert.IsType<IEnumerable<Game>>(result, exactMatch: false);
        Assert.Equal(2, returnedGames.Count());
    }

    [Fact]
    public async Task GetGameByIdAsync_ReturnsGame_WhenGameExists() {
        // Arrange
        const int gameId = 1;
        var game = new Game { Id = gameId, Title = "Test Game", Description = "Test Description", Price = 19.99m };
        _mockGameRepository.Setup(repo => repo.GetByIdAsync(gameId))
            .ReturnsAsync(game);

        // Act
        var result = await _gameService.GetGameByIdAsync(gameId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameId, result.Id);
    }

    [Fact]
    public async Task GetGameByIdAsync_ThrowsException_WhenGameDoesNotExist() {
        // Arrange
        const int gameId = 99;
        _mockGameRepository.Setup(repo => repo.GetByIdAsync(gameId))
            .ReturnsAsync((Game?)null);

        // Act & Assert
        await Assert.ThrowsAsync<GameNotFoundException>(() => _gameService.GetGameByIdAsync(gameId));
    }

    [Fact]
    public async Task CreateGameAsync_ReturnsCreatedGame_WhenTitleIsUnique() {
        // Arrange
        var gameDto = new GameDTO {
            Title = "New Game",
            Description = "New Description",
            Price = 39.99m
        };
        var createdGame = new Game {
            Id = 3,
            Title = gameDto.Title,
            Description = gameDto.Description,
            Price = gameDto.Price
        };
        _mockGameRepository.Setup(repo => repo.GetByTitleAsync(gameDto.Title))
            .ReturnsAsync((Game?)null);
        _mockGameRepository.Setup(repo => repo.CreateAsync(It.IsAny<Game>()))
            .ReturnsAsync(createdGame);

        // Act
        var result = await _gameService.CreateGameAsync(gameDto);

        // Assert
        Assert.Equal(createdGame.Id, result.Id);
        Assert.Equal(gameDto.Title, result.Title);
    }

    [Fact]
    public async Task CreateGameAsync_ThrowsException_WhenTitleAlreadyExists() {
        // Arrange
        var gameDto = new GameDTO {
            Title = "Existing Game",
            Description = "New Description",
            Price = 39.99m
        };
        var existingGame = new Game { Id = 1, Title = gameDto.Title };
        _mockGameRepository.Setup(repo => repo.GetByTitleAsync(gameDto.Title))
            .ReturnsAsync(existingGame);

        // Act & Assert
        await Assert.ThrowsAsync<GameAlreadyExistsException>(() => _gameService.CreateGameAsync(gameDto));
    }

    [Fact]
    public async Task UpdateGameAsync_ReturnsUpdatedGame_WhenGameExists() {
        // Arrange
        const int gameId = 1;
        var gameDto = new GameDTO {
            Title = "Updated Game",
            Description = "Updated Description",
            Price = 49.99m
        };
        var existingGame = new Game { Id = gameId, Title = "Test Game", Description = "Test Description", Price = 19.99m };
        var updatedGame = new Game {
            Id = gameId,
            Title = gameDto.Title,
            Description = gameDto.Description,
            Price = gameDto.Price
        };
        _mockGameRepository.Setup(repo => repo.GetByIdAsync(gameId))
            .ReturnsAsync(existingGame);
        _mockGameRepository.Setup(repo => repo.GetByTitleAsync(gameDto.Title))
            .ReturnsAsync((Game?)null);
        _mockGameRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Game>()))
            .ReturnsAsync(updatedGame);

        // Act
        var result = await _gameService.UpdateGameAsync(gameId, gameDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameId, result.Id);
        Assert.Equal(gameDto.Title, result.Title);
    }

    [Fact]
    public async Task UpdateGameAsync_ThrowsException_WhenGameDoesNotExist() {
        // Arrange
        const int gameId = 99;
        var gameDto = new GameDTO {
            Title = "Updated Game",
            Description = "Updated Description",
            Price = 49.99m
        };
        _mockGameRepository.Setup(repo => repo.GetByIdAsync(gameId))
            .ReturnsAsync((Game?)null);

        // Act & Assert
        await Assert.ThrowsAsync<GameNotFoundException>(() => _gameService.UpdateGameAsync(gameId, gameDto));
    }

    [Fact]
    public async Task DeleteGameAsync_DoesNotThrow_WhenGameExists() {
        // Arrange
        const int gameId = 1;
        var game = new Game { Id = gameId, Title = "Test Game", Description = "Test Description", Price = 19.99m };
        _mockGameRepository.Setup(repo => repo.GetByIdAsync(gameId))
            .ReturnsAsync(game);

        // Act & Assert
        await _gameService.DeleteGameAsync(gameId);
        _mockGameRepository.Verify(repo => repo.DeleteAsync(game), Times.Once);
    }

    [Fact]
    public async Task DeleteGameAsync_ThrowsException_WhenGameDoesNotExist() {
        // Arrange
        const int gameId = 99;
        _mockGameRepository.Setup(repo => repo.GetByIdAsync(gameId))
            .ReturnsAsync((Game?)null);

        // Act & Assert
        await Assert.ThrowsAsync<GameNotFoundException>(() => _gameService.DeleteGameAsync(gameId));
        _mockGameRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Game>()), Times.Never);
    }
}
