namespace EpamKse.GameStore.Api.Tests.Controllers;

using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;

using Api.Controllers;
using Domain.DTO.Game;
using Domain.Entities;
using Services.Services.Game;

public class GamesControllerTests {
    private readonly Mock<IGameService> _mockGameService;
    private readonly GamesController _controller;

    public GamesControllerTests() {
        _mockGameService = new Mock<IGameService>();
        _controller = new GamesController(_mockGameService.Object);
    }

    [Fact]
    public async Task GetAllGames_ReturnsOkWithGamesList() {
        var games = new List<Game> {
            new() { Id = 1, Title = "Strategy Game", GenreIds = [1, 2] },
            new() { Id = 2, Title = "Action Game", GenreIds = [4, 5] }
        };
        _mockGameService.Setup(s => s.GetAllGamesAsync()).ReturnsAsync(games);

        var result = await _controller.GetAllGames();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGames = Assert.IsAssignableFrom<IEnumerable<Game>>(okResult.Value);
        Assert.Equal(2, returnedGames.Count());
    }

    [Fact]
    public async Task GetGameById_ReturnsGameWithGenres() {
        var game = new Game { Id = 1, Title = "Test Game", GenreIds = [1, 2, 4] };
        _mockGameService.Setup(s => s.GetGameByIdAsync(1)).ReturnsAsync(game);

        var result = await _controller.GetGameById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGame = Assert.IsType<Game>(okResult.Value);
        Assert.Equal(1, returnedGame.Id);
        Assert.Equal(3, returnedGame.GenreIds.Count);
    }

    [Fact]
    public async Task CreateGame_WithGenresAndSubgenres_ReturnsCreated() {
        var gameDto = new GameDto {
            Title = "New Game", Description = "Description", Price = 39.99m, ReleaseDate = DateTime.Now,
            GenreNames = ["Strategy", "Action"], SubGenreNames = ["RTS", "FPS"]
        };
        var createdGame = new Game { 
            Id = 3, Title = gameDto.Title, GenreIds = [1, 2, 4, 5] 
        };
        _mockGameService.Setup(s => s.CreateGameAsync(gameDto)).ReturnsAsync(createdGame);

        var result = await _controller.CreateGame(gameDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedGame = Assert.IsType<Game>(createdResult.Value);
        Assert.Equal("New Game", returnedGame.Title);
        Assert.Equal(4, returnedGame.GenreIds.Count);
    }

    [Fact]
    public async Task CreateGame_OnlyMainGenres_ReturnsCreated() {
        var gameDto = new GameDto {
            Title = "Simple Game", Description = "Description", Price = 19.99m, ReleaseDate = DateTime.Now,
            GenreNames = ["Sports"], SubGenreNames = []
        };
        var createdGame = new Game { 
            Id = 4, Title = gameDto.Title, GenreIds = [6] 
        };
        _mockGameService.Setup(s => s.CreateGameAsync(gameDto)).ReturnsAsync(createdGame);

        var result = await _controller.CreateGame(gameDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedGame = Assert.IsType<Game>(createdResult.Value);
        Assert.Single(returnedGame.GenreIds);
    }

    [Fact]
    public async Task UpdateGame_ChangeGenres_ReturnsUpdated() {
        var gameDto = new GameDto {
            Title = "Updated Game", Description = "Updated", Price = 49.99m, ReleaseDate = DateTime.Now,
            GenreNames = ["Action"], SubGenreNames = ["FPS"]
        };
        var updatedGame = new Game { 
            Id = 1, Title = gameDto.Title, GenreIds = [4, 5] 
        };
        _mockGameService.Setup(s => s.UpdateGameAsync(1, gameDto)).ReturnsAsync(updatedGame);

        var result = await _controller.UpdateGame(1, gameDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGame = Assert.IsType<Game>(okResult.Value);
        Assert.Equal("Updated Game", returnedGame.Title);
        Assert.Equal(2, returnedGame.GenreIds.Count);
    }

    [Fact]
    public async Task DeleteGame_ValidId_ReturnsNoContent() {
        _mockGameService.Setup(s => s.DeleteGameAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteGame(1);

        Assert.IsType<NoContentResult>(result);
        _mockGameService.Verify(s => s.DeleteGameAsync(1), Times.Once);
    }
}
