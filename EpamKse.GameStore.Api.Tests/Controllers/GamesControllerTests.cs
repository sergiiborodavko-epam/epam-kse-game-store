namespace EpamKse.GameStore.Api.Tests.Controllers;

using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;

using Api.Controllers;
using Domain.DTO;
using Domain.Entities;
using Domain.Exceptions;
using Services.Services.Game;

public class GamesControllerTests {
    private readonly Mock<IGameService> _mockGameService;
    private readonly GamesController _controller;

    public GamesControllerTests() {
        _mockGameService = new Mock<IGameService>();
        _controller = new GamesController(_mockGameService.Object);
    }

    [Fact]
    public async Task GetAllGames_ReturnsOkResult_WithListOfGames() {
        // Arrange
        var games = new List<Game> {
            new() { Id = 1, Title = "Test Game 1", Description = "Test Description 1", Price = 19.99m },
            new() { Id = 2, Title = "Test Game 2", Description = "Test Description 2", Price = 29.99m }
        };
        _mockGameService.Setup(service => service.GetAllGamesAsync())
            .ReturnsAsync(games);

        // Act
        var result = await _controller.GetAllGames();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGames = Assert.IsType<IEnumerable<Game>>(okResult.Value, exactMatch: false);
        Assert.Equal(2, returnedGames.Count());
    }

    [Fact]
    public async Task GetGameById_ReturnsOkResult_WithGame_WhenGameExists() {
        // Arrange
        const int gameId = 1;
        var game = new Game { Id = gameId, Title = "Test Game", Description = "Test Description", Price = 19.99m };
        _mockGameService.Setup(service => service.GetGameByIdAsync(gameId))
            .ReturnsAsync(game);

        // Act
        var result = await _controller.GetGameById(gameId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGame = Assert.IsType<Game>(okResult.Value);
        Assert.Equal(gameId, returnedGame.Id);
    }

    [Fact]
    public async Task GetGameById_ReturnsNotFound_WhenGameDoesNotExist() {
        // Arrange
        const int gameId = 99;
        _mockGameService.Setup(service => service.GetGameByIdAsync(gameId))
            .ThrowsAsync(new GameNotFoundException(gameId));

        // Act
        var result = await _controller.GetGameById(gameId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateGame_ReturnsCreatedAtAction_WithNewGame() {
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
        _mockGameService.Setup(service => service.CreateGameAsync(gameDto))
            .ReturnsAsync(createdGame);

        // Act
        var result = await _controller.CreateGame(gameDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedGame = Assert.IsType<Game>(createdAtActionResult.Value);
        Assert.Equal(createdGame.Id, returnedGame.Id);
        Assert.Equal(gameDto.Title, returnedGame.Title);
    }

    [Fact]
    public async Task CreateGame_ReturnsConflict_WhenGameAlreadyExists() {
        // Arrange
        var gameDto = new GameDTO {
            Title = "Existing Game",
            Description = "New Description",
            Price = 39.99m
        };
        _mockGameService.Setup(service => service.CreateGameAsync(gameDto))
            .ThrowsAsync(new GameAlreadyExistsException(gameDto.Title));

        // Act
        var result = await _controller.CreateGame(gameDto);

        // Assert
        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task UpdateGame_ReturnsOkResult_WithUpdatedGame_WhenGameExists()
    {
        // Arrange
        const int gameId = 1;
        var gameDto = new GameDTO {
            Title = "Updated Game",
            Description = "Updated Description",
            Price = 49.99m
        };
        var updatedGame = new Game {
            Id = gameId,
            Title = gameDto.Title,
            Description = gameDto.Description,
            Price = gameDto.Price
        };
        _mockGameService.Setup(service => service.UpdateGameAsync(gameId, gameDto))
            .ReturnsAsync(updatedGame);

        // Act
        var result = await _controller.UpdateGame(gameId, gameDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGame = Assert.IsType<Game>(okResult.Value);
        Assert.Equal(gameId, returnedGame.Id);
        Assert.Equal(gameDto.Title, returnedGame.Title);
    }

    [Fact]
    public async Task UpdateGame_ReturnsNotFound_WhenGameDoesNotExist() {
        // Arrange
        const int gameId = 99;
        var gameDto = new GameDTO {
            Title = "Updated Game",
            Description = "Updated Description",
            Price = 49.99m
        };
        _mockGameService.Setup(service => service.UpdateGameAsync(gameId, gameDto))
            .ThrowsAsync(new GameNotFoundException(gameId));

        // Act
        var result = await _controller.UpdateGame(gameId, gameDto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteGame_ReturnsNoContent_WhenGameExists() {
        // Arrange
        var gameId = 1;
        _mockGameService.Setup(service => service.DeleteGameAsync(gameId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteGame(gameId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteGame_ReturnsNotFound_WhenGameDoesNotExist() {
        // Arrange
        const int gameId = 99;
        _mockGameService.Setup(service => service.DeleteGameAsync(gameId))
            .ThrowsAsync(new GameNotFoundException(gameId));

        // Act
        var result = await _controller.DeleteGame(gameId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
