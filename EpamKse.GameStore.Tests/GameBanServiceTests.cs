namespace EpamKse.GameStore.Tests;

using Xunit;
using Moq;

using DataAccess.Repositories.Game;
using DataAccess.Repositories.GameBan;
using Domain.DTO.GameBan;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions.Game;
using Domain.Exceptions.GameBan;
using EpamKse.GameStore.Services.Services.GameBan;

public class GameBanServiceTests {
    private readonly Mock<IGameBanRepository> _mockBanRepo;
    private readonly Mock<IGameRepository> _mockGameRepo;
    private readonly GameBanService _service;

    public GameBanServiceTests() {
        _mockBanRepo = new Mock<IGameBanRepository>();
        _mockGameRepo = new Mock<IGameRepository>();
        _service = new GameBanService(_mockBanRepo.Object, _mockGameRepo.Object);
    }

    [Fact]
    public async Task GetAllBansAsync_ReturnsAllBans() {
        var bans = new List<GameBan>
        {
            new() { Id = 1, GameId = 1, Country = Countries.UA, Game = new Game { Title = "Game 1" } },
            new() { Id = 2, GameId = 2, Country = Countries.US, Game = new Game { Title = "Game 2" } }
        };
        _mockBanRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(bans);

        var result = await _service.GetAllBansAsync();

        var bansList = result.ToList();
        Assert.Equal(2, bansList.Count);
        Assert.Equal("Game 1", bansList[0].GameTitle);
        Assert.Equal(Countries.UA, bansList[0].Country);
    }

    [Fact]
    public async Task GetBanByIdAsync_ExistingBan_ReturnsBan() {
        var ban = new GameBan { 
            Id = 1, 
            GameId = 1, 
            Country = Countries.UA, 
            Game = new Game { Title = "Test Game" },
            CreatedAt = DateTime.UtcNow
        };
        _mockBanRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ban);

        var result = await _service.GetBanByIdAsync(1);

        Assert.Equal("Test Game", result.GameTitle);
        Assert.Equal(Countries.UA, result.Country);
    }

    [Fact]
    public async Task GetBanByIdAsync_NonExistentBan_ThrowsNotFoundException() {
        _mockBanRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((GameBan?)null);

        await Assert.ThrowsAsync<BanNotFoundException>(() => _service.GetBanByIdAsync(999));
    }

    [Fact]
    public async Task GetBansByCountryAsync_ValidCountry_ReturnsCountryBans() {
        const string country = "UA";
        var bans = new List<GameBan>
        {
            new() { Id = 1, GameId = 1, Country = Countries.UA, Game = new Game { Title = "Game 1" } },
            new() { Id = 2, GameId = 2, Country = Countries.UA, Game = new Game { Title = "Game 2" } }
        };
        _mockBanRepo.Setup(r => r.GetByCountryAsync(Countries.UA)).ReturnsAsync(bans);

        var result = await _service.GetBansByCountryAsync(country);

        var bansList = result.ToList();
        Assert.Equal(2, bansList.Count);
        Assert.All(bansList, b => Assert.Equal(Countries.UA, b.Country));
    }

    [Fact]
    public async Task CreateBanAsync_ValidData_CreatesBan() {
        var dto = new CreateGameBanDto { GameId = 1, Country = "UA" };
        var game = new Game { Id = 1, Title = "Test Game" };
        var createdBan = new GameBan { 
            Id = 1, 
            GameId = 1, 
            Country = Countries.UA, 
            Game = game,
            CreatedAt = DateTime.UtcNow
        };

        _mockGameRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(game);
        _mockBanRepo.Setup(r => r.GetByGameAndCountryAsync(1, Countries.UA)).ReturnsAsync((GameBan?)null);
        _mockBanRepo.Setup(r => r.CreateAsync(It.IsAny<GameBan>())).ReturnsAsync(createdBan);

        var result = await _service.CreateBanAsync(dto);

        Assert.Equal("Test Game", result.GameTitle);
        Assert.Equal(Countries.UA, result.Country);
        _mockBanRepo.Verify(r => r.CreateAsync(It.IsAny<GameBan>()), Times.Once);
    }

    [Fact]
    public async Task CreateBanAsync_InvalidCountry_ThrowsInvalidCountryException() {
        var dto = new CreateGameBanDto { GameId = 1, Country = "INVALID" };

        await Assert.ThrowsAsync<InvalidCountryException>(() => _service.CreateBanAsync(dto));
    }

    [Fact]
    public async Task CreateBanAsync_GameNotFound_ThrowsGameNotFoundException() {
        var dto = new CreateGameBanDto { GameId = 999, Country = "UA" };
        _mockGameRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Game?)null);

        await Assert.ThrowsAsync<GameNotFoundException>(() => _service.CreateBanAsync(dto));
    }

    [Fact]
    public async Task CreateBanAsync_BanAlreadyExists_ThrowsBanAlreadyExistsException() {
        var dto = new CreateGameBanDto { GameId = 1, Country = "UA" };
        var game = new Game { Id = 1, Title = "Test Game" };
        var existingBan = new GameBan { Id = 1, GameId = 1, Country = Countries.UA };

        _mockGameRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(game);
        _mockBanRepo.Setup(r => r.GetByGameAndCountryAsync(1, Countries.UA)).ReturnsAsync(existingBan);

        await Assert.ThrowsAsync<BanAlreadyExistsException>(() => _service.CreateBanAsync(dto));
    }

    [Fact]
    public async Task DeleteBanAsync_ExistingBan_DeletesBan() {
        var ban = new GameBan { Id = 1, GameId = 1, Country = Countries.UA };
        _mockBanRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ban);

        await _service.DeleteBanAsync(1);

        _mockBanRepo.Verify(r => r.DeleteAsync(ban), Times.Once);
    }

    [Fact]
    public async Task DeleteBanAsync_NonExistentBan_ThrowsNotFoundException() {
        _mockBanRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((GameBan?)null);

        await Assert.ThrowsAsync<BanNotFoundException>(() => _service.DeleteBanAsync(999));
    }
}
