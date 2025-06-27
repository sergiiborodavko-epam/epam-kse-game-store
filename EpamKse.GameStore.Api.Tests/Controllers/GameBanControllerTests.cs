using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;

using EpamKse.GameStore.Api.Controllers;
using EpamKse.GameStore.Domain.DTO.GameBan;
using EpamKse.GameStore.Domain.Enums;
using EpamKse.GameStore.Services.Services.GameBan;

namespace EpamKse.GameStore.Api.Tests.Controllers;

public class GameBanControllerTests {
    private readonly Mock<IGameBanService> _mockService;
    private readonly GameBanController _controller;

    public GameBanControllerTests() {
        _mockService = new Mock<IGameBanService>();
        _controller = new GameBanController(_mockService.Object);
    }

    [Fact]
    public async Task GetAllBans_ReturnsOkWithBansList() {
        var bans = new List<GameBanDto> {
            new() { Id = 1, GameId = 1, GameTitle = "Game 1", Country = Countries.UA },
            new() { Id = 2, GameId = 2, GameTitle = "Game 2", Country = Countries.US }
        };
        _mockService.Setup(s => s.GetAllBansAsync()).ReturnsAsync(bans);

        var result = await _controller.GetAllBans();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBans = Assert.IsType<IEnumerable<GameBanDto>>(okResult.Value, exactMatch: false);
        Assert.Equal(2, returnedBans.Count());
    }

    [Fact]
    public async Task GetBanById_ReturnsBanDetails() {
        var ban = new GameBanDto {
            Id = 1,
            GameId = 1,
            GameTitle = "Test Game",
            Country = Countries.UA,
            CreatedAt = DateTime.UtcNow
        };
        _mockService.Setup(s => s.GetBanByIdAsync(1)).ReturnsAsync(ban);

        var result = await _controller.GetBanById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBan = Assert.IsType<GameBanDto>(okResult.Value);
        Assert.Equal("Test Game", returnedBan.GameTitle);
        Assert.Equal(Countries.UA, returnedBan.Country);
    }

    [Fact]
    public async Task GetBansByCountry_ReturnsCountryBans() {
        const string country = "UA";
        var bans = new List<GameBanDto> {
            new() { Id = 1, GameId = 1, GameTitle = "Game 1", Country = Countries.UA },
            new() { Id = 2, GameId = 2, GameTitle = "Game 2", Country = Countries.UA }
        };
        _mockService.Setup(s => s.GetBansByCountryAsync(country)).ReturnsAsync(bans);

        var result = await _controller.GetBansByCountry(country);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBans = Assert.IsType<IEnumerable<GameBanDto>>(okResult.Value, exactMatch: false);
        var bansList = returnedBans.ToList();
        Assert.Equal(2, bansList.Count);
        Assert.All(bansList, b => Assert.Equal(Countries.UA, b.Country));
    }

    [Fact]
    public async Task CreateBan_ValidData_ReturnsCreated() {
        var dto = new CreateGameBanDto {
            GameId = 1,
            Country = "UA"
        };
        var createdBan = new GameBanDto {
            Id = 1,
            GameId = 1,
            GameTitle = "Test Game",
            Country = Countries.UA,
            CreatedAt = DateTime.UtcNow
        };
        _mockService.Setup(s => s.CreateBanAsync(dto)).ReturnsAsync(createdBan);

        var result = await _controller.CreateBan(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedBan = Assert.IsType<GameBanDto>(createdResult.Value);
        Assert.Equal("Test Game", returnedBan.GameTitle);
        Assert.Equal(Countries.UA, returnedBan.Country);
    }

    [Fact]
    public async Task DeleteBan_ValidId_ReturnsNoContent() {
        _mockService.Setup(s => s.DeleteBanAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteBan(1);

        Assert.IsType<NoContentResult>(result);
        _mockService.Verify(s => s.DeleteBanAsync(1), Times.Once);
    }
}