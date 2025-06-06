using EpamKse.GameStore.DataAccess.Repositories.Game;
using EpamKse.GameStore.DataAccess.Repositories.HistoricalPrice;
using EpamKse.GameStore.Services.Services.Game;
using EpamKse.GameStore.DataAccess.Context;
using EpamKse.GameStore.DataAccess.Repositories.Genre;
using EpamKse.GameStore.Domain.DTO.Game;
using EpamKse.GameStore.Domain.Entities;
using EpamKse.GameStore.Services.Services.HistoricalPrice;


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

    public HistoricalPriceTest()
    {
        var options = new DbContextOptionsBuilder<GameStoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new GameStoreDbContext(options);
        var gameRepo = new GameRepository(_context);
        var priceRepo = new HistoricalPriceRepository(_context);
        var genreRepository = new GenreRepository(_context);
        _gameService = new GameService(gameRepo, genreRepository, priceRepo);
        _historicalPriceService = new HistoricalPriceService(priceRepo);
    }

    [Fact]
    public async Task CreateGame_ShouldCreateHistoricalPrice()
    {
        var dto = new GameDto
        {
            Title = "Game A",
            Price = 100,
            Description = "A",
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
        var game = new GameDto
        {
            Title = "Game B",
            Price = 200,
            Description = "start",
            ReleaseDate = DateTime.UtcNow
        };
        var createdGame = await _gameService.CreateGameAsync(game);


        var dto = new GameDto
        {
            Title = "Game B",
            Price = 250,
            Description = "Updated",
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
        var game = new GameDto
        {
            Title = "Game C",
            Price = 300,
            Description = "original",
            ReleaseDate = DateTime.UtcNow
        };
        var createdGame = await _gameService.CreateGameAsync(game);

        var dto = new GameDto
        {
            Title = "Game C",
            Price = 300,
            Description = "still original",
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
}