using EpamKse.GameStore.DataAccess.Context;
using EpamKse.GameStore.DataAccess.Repositories.Game;
using EpamKse.GameStore.DataAccess.Repositories.Genre;
using EpamKse.GameStore.DataAccess.Repositories.HistoricalPrice;
using EpamKse.GameStore.DataAccess.Repositories.Platform;
using EpamKse.GameStore.DataAccess.Repositories.Publisher;
using EpamKse.GameStore.Domain.DTO.Game;
using EpamKse.GameStore.Domain.DTO.Publisher;
using EpamKse.GameStore.Domain.Entities;
using EpamKse.GameStore.Domain.Exceptions.Game;
using EpamKse.GameStore.Domain.Exceptions.Publisher;
using EpamKse.GameStore.Services.Services.Game;
using EpamKse.GameStore.Services.Services.Publisher;

namespace EpamKse.GameStore.Tests;

using Microsoft.EntityFrameworkCore;
using Xunit;

public class GameServiceTests
{
    private readonly GameStoreDbContext _dbContext;
    private readonly GameService _gameService;
    private readonly PublisherService _pubService;

    public GameServiceTests()
    {
        var options = new DbContextOptionsBuilder<GameStoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new GameStoreDbContext(options);
        var gameRepository = new GameRepository(_dbContext);
        var genreRepository = new GenreRepository(_dbContext);
        var historicalPriceRepository = new HistoricalPriceRepository(_dbContext);
        var publisherRepository = new PublisherRepository(_dbContext);
        var platformRepository = new PlatformRepository(_dbContext);
        _gameService = new GameService(gameRepository, genreRepository, historicalPriceRepository, publisherRepository);
        _pubService = new PublisherService(publisherRepository, platformRepository);
    }

    [Fact]
    public async Task SetPublisherToGame_ShouldAssignPublisher_WhenValidIds()
    {
        var createPublisher = new CreatePublisherDTO
        {
            Name = "New Publisher",
            Description = "Great games",
            HomePageUrl = "https://example.com"
        };

        var publisher = await _pubService.CreatePublisher(createPublisher);

        var game = new Game { Title = "TestGame", Price = 10 };
        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();

        var dto = new SetPublisherDto { gameId = game.Id, publisherId = publisher.Id };
        var result = await _gameService.SetPublisherToGame(dto);
        var updatedGame = await _dbContext.Games.FindAsync(game.Id);
        Assert.True(result);
        Assert.Equal(publisher.Id, updatedGame.PublisherId);
    }

    [Fact]
    public async Task SetPublisherToGame_ShouldThrow_WhenGameNotFound()
    {
        var createPublisher = new CreatePublisherDTO
        {
            Name = "New Publisher",
            Description = "Great games",
            HomePageUrl = "https://example.com"
        };

        var result = await _pubService.CreatePublisher(createPublisher);

        var dto = new SetPublisherDto { gameId = 999, publisherId = result.Id };
        await Assert.ThrowsAsync<GameNotFoundException>(() => _gameService.SetPublisherToGame(dto));
    }

    [Fact]
    public async Task SetPublisherToGame_ShouldThrow_WhenPublisherNotFound()
    {
        var game = new Game { Title = "GameOnly", Price = 20 };
        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();

        var dto = new SetPublisherDto { gameId = game.Id, publisherId = 999 };
        await Assert.ThrowsAsync<PublisherNotFoundException>(() => _gameService.SetPublisherToGame(dto));
    }

    [Fact]
    public async Task CreateGameAsync_ShouldCreateGame_WithValidPublisher()
    {
        var createPublisher = new CreatePublisherDTO
        {
            Name = "Epic Games",
            Description = "AAA Publisher",
            HomePageUrl = "https://epicgames.com"
        };
        var publisher = await _pubService.CreatePublisher(createPublisher);

        var gameDto = new GameDto
        {
            Title = "Game",
            Description = "Descr",
            Price = 10,
            PublisherId = publisher.Id
        };
        var createdGame = await _gameService.CreateGameAsync(gameDto);

        var foundedGame = await _dbContext.Games.FindAsync(createdGame.Id);
        Assert.NotNull(createdGame);
        Assert.Equal(gameDto.Title, createdGame.Title);
        Assert.Equal(publisher.Id, foundedGame.PublisherId);
    }

    [Fact]
    public async Task UpdateGameAsync_ShouldUpdateGame_WhenValidPublisherId()
    {
        var createPublisher = new CreatePublisherDTO
        {
            Name = "Initial Publisher",
            Description = "Publisher for old game",
            HomePageUrl = "https://old.com"
        };
        var oldPublisher = await _pubService.CreatePublisher(createPublisher);

        var newPublisherDto = new CreatePublisherDTO
        {
            Name = "New Publisher",
            Description = "New Publisher Info",
            HomePageUrl = "https://new.com"
        };
        var newPublisher = await _pubService.CreatePublisher(newPublisherDto);

        var game = new GameDto
        {
            Title = "Update Me",
            Description = "Old description",
            Price = 49,
            PublisherId = oldPublisher.Id,
        };
        var createdGame = await _gameService.CreateGameAsync(game);

        var updateDto = new GameDto
        {
            Title = "Update Me",
            PublisherId = newPublisher.Id,
            Price = 49
        };
        var updatedGameDto = await _gameService.UpdateGameAsync(createdGame.Id, updateDto);
        Assert.NotNull(updatedGameDto);
        var updatedGame = await _dbContext.Games.FindAsync(createdGame.Id);
        Assert.Equal(newPublisher.Id, updatedGame.PublisherId);
    }

    [Fact]
    public async Task UpdateGameAsync_ShouldThrow_WhenPublisherDoesNotExist()
    {
        var createPublisher = new CreatePublisherDTO
        {
            Name = "Initial Publisher",
            Description = "Publisher for old game",
            HomePageUrl = "https://old.com"
        };
        var publisher = await _pubService.CreatePublisher(createPublisher);
        var game = new GameDto
        {
            Title = "Update Me",
            Description = "Old description",
            Price = 49,
            PublisherId = publisher.Id,
        };
        var createdGame = await _gameService.CreateGameAsync(game);

        var updateDto = new GameDto
        {
            Title = "Update Me",
            PublisherId = 11111,
            Price = 49
        };
        await Assert.ThrowsAsync<PublisherNotFoundException>(() =>
            _gameService.UpdateGameAsync(createdGame.Id, updateDto));
    }


    [Fact]
    public async Task CreateGameAsync_ShouldThrow_WhenPublisherDoesNotExist()
    {
        var gameDto = new GameDto
        {
            Title = "Game",
            Description = "Descr",
            Price = 10,
            PublisherId = 22
        };
        await Assert.ThrowsAsync<PublisherNotFoundException>(() =>
            _gameService.CreateGameAsync(gameDto));
    }
}