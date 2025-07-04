using EpamKse.GameStore.DataAccess.Repositories.Game;
using EpamKse.GameStore.DataAccess.Repositories.HistoricalPrice;
using EpamKse.GameStore.Services.Services.Game;
using EpamKse.GameStore.DataAccess.Context;
using EpamKse.GameStore.DataAccess.Repositories.Genre;
using EpamKse.GameStore.DataAccess.Repositories.Platform;
using EpamKse.GameStore.DataAccess.Repositories.Publisher;
using EpamKse.GameStore.Domain.DTO.Game;
using EpamKse.GameStore.Domain.DTO.Publisher;
using EpamKse.GameStore.Domain.Entities;
using EpamKse.GameStore.Domain.Exceptions;
using EpamKse.GameStore.Services.Services.HistoricalPrice;
using EpamKse.GameStore.Services.Services.Publisher;


namespace EpamKse.GameStore.Tests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class HistoricalPriceTest
{
    private readonly GameStoreDbContext _context;
    private readonly GameService _gameService;
    private readonly HistoricalPriceService _historicalPriceService;
    private PublisherService _pubService;
    public HistoricalPriceTest()
    {
        var options = new DbContextOptionsBuilder<GameStoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new GameStoreDbContext(options);
        var gameRepo = new GameRepository(_context);
        var priceRepo = new HistoricalPriceRepository(_context);
        var genreRepository = new GenreRepository(_context);
        var publisherRepository = new PublisherRepository(_context);
        var platformRepository = new PlatformRepository(_context);
        _pubService = new PublisherService(publisherRepository,platformRepository);
        _gameService = new GameService(gameRepo, genreRepository, priceRepo, publisherRepository);
        _historicalPriceService = new HistoricalPriceService(priceRepo);
    }

    [Fact]
    public async Task CreateGame_ShouldCreateHistoricalPrice()
    {
        var createPublisher = new CreatePublisherDTO
        {
            Name = "Initial Publisher",
            Description = "Publisher for game",
            HomePageUrl = "https://old.com"
        };
        var publisher = await _pubService.CreatePublisher(createPublisher);
        var dto = new GameDto
        {
            Title = "Game A",
            Price = 100,
            Description = "A",
            Stock = 12,
            PublisherId = publisher.Id,
            ReleaseDate = DateTime.UtcNow
        };

        var result = await _gameService.CreateGameAsync(dto);

        var prices = await _context.HistoricalPrices.ToListAsync();

        Assert.Single(prices);
        Assert.Equal(100, prices[0].Price);
        Assert.Equal(result.Id, prices[0].GameId);
    }

    [Fact]
    public async Task UpdateGame_PriceChanged_CreatesNewHistoricalPrice()
    {
        var createPublisher = new CreatePublisherDTO
        {
            Name = "Initial Publisher",
            Description = "Publisher for game",
            HomePageUrl = "https://old.com"
        };
        var publisher = await _pubService.CreatePublisher(createPublisher);
        var game = new GameDto
        {
            Title = "Game B",
            Price = 200,
            Description = "start",
            Stock = 12,
            PublisherId = publisher.Id,
            ReleaseDate = DateTime.UtcNow
        };
        var createdGame = await _gameService.CreateGameAsync(game);


        var dto = new GameDto
        {
            Title = "Game B",
            Price = 250,
            Stock = 12,
            Description = "Updated",
            PublisherId = publisher.Id,
            ReleaseDate = DateTime.UtcNow
        };

        await _gameService.UpdateGameAsync(createdGame.Id, dto);

        var prices = await _context.HistoricalPrices
            .Where(p => p.GameId == createdGame.Id)
            .ToListAsync();


        Assert.Equal(250, prices[1].Price);
    }

    [Fact]
    public async Task UpdateGame_PriceUnchanged_DoesNotCreateNewHistoricalPrice()
    {
        var createPublisher = new CreatePublisherDTO
        {
            Name = "Initial Publisher",
            Description = "Publisher for game",
            HomePageUrl = "https://old.com"
        };
        var publisher = await _pubService.CreatePublisher(createPublisher);
        var game = new GameDto
        {
            Title = "Game C",
            Price = 300,
            Stock = 12,
            Description = "original",
            PublisherId = publisher.Id,
            ReleaseDate = DateTime.UtcNow
        };
        var createdGame = await _gameService.CreateGameAsync(game);

        var dto = new GameDto
        {
            Title = "Game C",
            Price = 300,
            Stock = 12,
            Description = "still original",
            PublisherId = publisher.Id,
            ReleaseDate = DateTime.UtcNow
        };

        await _gameService.UpdateGameAsync(createdGame.Id, dto);

        var prices = await _context.HistoricalPrices
            .Where(p => p.GameId == createdGame.Id)
            .ToListAsync();

        Assert.Single(prices);
    }

    [Fact]
    public async Task GetPaginatedPrices_ReturnsPaginatedList()
    {
        var gameId = 1;
        var price1 = new HistoricalPrice
        {
            GameId = gameId,
            Price = 100,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };
        var price2 = new HistoricalPrice
        {
            GameId = gameId,
            Price = 200,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        await _context.HistoricalPrices.AddRangeAsync(price1, price2);
        await _context.SaveChangesAsync();

        var result = await _historicalPriceService.GetPricesForGame(id: gameId, page: 2, limit: 1);

        Assert.Single(result);
        Assert.Equal(200, result[0].Price);
        Assert.Equal(gameId, result[0].GameId);
    }
    
    [Fact]
    public async Task GetPricesForGame_WithoutPagination_ReturnsAllPrices()
    {
        var gameId = 2;
        var price1 = new HistoricalPrice
        {
            GameId = gameId,
            Price = 150,
            CreatedAt = DateTime.UtcNow.AddDays(-3)
        };
        var price2 = new HistoricalPrice
        {
            GameId = gameId,
            Price = 180,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };
        var price3 = new HistoricalPrice
        {
            GameId = gameId,
            Price = 210,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        await _context.HistoricalPrices.AddRangeAsync(price1, price2, price3);
        await _context.SaveChangesAsync();

        var result = await _historicalPriceService.GetPricesForGame(id: gameId, page: null, limit: null);

        Assert.Equal(3, result.Count);
        Assert.All(result, r => Assert.Equal(gameId, r.GameId));
    }
    
    [Fact]
    public async Task GetPricesForGame_PageOnly_ThrowsInvalidPaginationException()
    {
        var gameId = 3;

        var ex = await Assert.ThrowsAsync<InvalidPaginationException>(() =>
            _historicalPriceService.GetPricesForGame(id: gameId, page: 1, limit: null));

        Assert.Equal("Page and limit must be present and greater than zero.", ex.Message);
    }
}