using EpamKse.GameStore.DataAccess.Repositories.HistoricalPrice;
using EpamKse.GameStore.DataAccess.Repositories.Platform;
using EpamKse.GameStore.DataAccess.Repositories.Publisher;
using EpamKse.GameStore.Domain.DTO.Publisher;
using EpamKse.GameStore.Services.Services.Publisher;

namespace EpamKse.GameStore.Tests;

using Microsoft.EntityFrameworkCore;

using EpamKse.GameStore.Services.Services.Game;

using DataAccess.Repositories.Game;
using DataAccess.Repositories.Genre;
using DataAccess.Context;

using Domain.DTO.Game;
using Domain.Exceptions.Genre;
using Domain.Entities;

public class GenreValidationTests {
    private GameStoreDbContext _context;
    private GameService _gameService;
    private PublisherService _pubService;
    [SetUp]
    public void Setup() {
        var options = new DbContextOptionsBuilder<GameStoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new GameStoreDbContext(options);
        SeedGenres();
        
        var gameRepository = new GameRepository(_context);
        var genreRepository = new GenreRepository(_context);
        var historicalPriceRepository = new HistoricalPriceRepository(_context);
        var publisherRepository = new PublisherRepository(_context);
        var platformRepository = new PlatformRepository(_context);
        _pubService = new PublisherService(publisherRepository,platformRepository);
        _gameService = new GameService(gameRepository, genreRepository, historicalPriceRepository,publisherRepository);
    }

    private void SeedGenres()
    {
        _context.Genres.AddRange(
            new Genre { Id = 1, Name = "Strategy", ParentGenreId = null },
            new Genre { Id = 2, Name = "RTS", ParentGenreId = 1 },
            new Genre { Id = 3, Name = "TBS", ParentGenreId = 1 },
            new Genre { Id = 4, Name = "Action", ParentGenreId = null },
            new Genre { Id = 5, Name = "FPS", ParentGenreId = 4 },
            new Genre { Id = 6, Name = "Sports", ParentGenreId = null }
        );
        _context.SaveChanges();
    }

    [Test]
    public async Task CreateGame_ValidGenresWithSubgenres_Success() {
        var createPublisher = new CreatePublisherDTO
        {
            Name = "Initial Publisher",
            Description = "Publisher for game",
            HomePageUrl = "https://old.com"
        };
        var publisher = await _pubService.CreatePublisher(createPublisher);
        var gameDto = new GameDto {
            Title = "Test Game", Description = "Test", Price = 19.99m, ReleaseDate = DateTime.Now,Stock = 12, PublisherId = publisher.Id,
            GenreNames = ["Strategy", "Action"], SubGenreNames = ["RTS", "FPS"]
        };

        var result = await _gameService.CreateGameAsync(gameDto);

        Assert.That(result.GenreIds, Has.Count.EqualTo(4));
        Assert.That(result.GenreIds, Contains.Item(1)); // Strategy
        Assert.That(result.GenreIds, Contains.Item(2)); // RTS
        Assert.That(result.GenreIds, Contains.Item(4)); // Action  
        Assert.That(result.GenreIds, Contains.Item(5)); // FPS
    }

    [Test]
    public async Task CreateGame_OnlyMainGenres_Success() {
        var createPublisher = new CreatePublisherDTO
        {
            Name = "Initial Publisher",
            Description = "Publisher for game",
            HomePageUrl = "https://old.com"
        };
        var publisher = await _pubService.CreatePublisher(createPublisher);
        var gameDto = new GameDto {
            Title = "Test Game", Description = "Test", Stock = 12, Price = 19.99m, ReleaseDate = DateTime.Now, PublisherId = publisher.Id,
            GenreNames = ["Strategy", "Sports"], SubGenreNames = []
        };

        var result = await _gameService.CreateGameAsync(gameDto);

        Assert.That(result.GenreIds, Has.Count.EqualTo(2));
        Assert.That(result.GenreIds, Contains.Item(1)); // Strategy
        Assert.That(result.GenreIds, Contains.Item(6)); // Sports
    }

    [Test]
    public async Task CreateGame_SubgenreWithoutParent_ThrowsException() {
        var createPublisher = new CreatePublisherDTO
        {
            Name = "Initial Publisher",
            Description = "Publisher for game",
            HomePageUrl = "https://old.com"
        };
        var publisher = await _pubService.CreatePublisher(createPublisher);
        var gameDto = new GameDto {
            Title = "Test Game", Description = "Test", Stock = 12, Price = 19.99m, ReleaseDate = DateTime.Now, PublisherId = publisher.Id,
            GenreNames = ["Sports"], SubGenreNames = ["RTS"] // RTS belongs to Strategy, not Sports
        };

        Assert.ThrowsAsync<SubgenreWithoutParentException>(() => _gameService.CreateGameAsync(gameDto));
    }

    [Test]
    public async Task CreateGame_NonExistentGenre_ThrowsException() {
        var createPublisher = new CreatePublisherDTO
        {
            Name = "Initial Publisher",
            Description = "Publisher for game",
            HomePageUrl = "https://old.com"
        };
        var publisher = await _pubService.CreatePublisher(createPublisher);
        var gameDto = new GameDto {
            Title = "Test Game", Description = "Test", Stock = 12, Price = 19.99m, ReleaseDate = DateTime.Now, PublisherId = publisher.Id,
            GenreNames = ["NonExistent"], SubGenreNames = []
        };

        Assert.ThrowsAsync<GenresNotFoundException>(() => _gameService.CreateGameAsync(gameDto));
    }

    [Test]
    public async Task CreateGame_NonExistentSubgenre_ThrowsException() {
        var createPublisher = new CreatePublisherDTO
        {
            Name = "Initial Publisher",
            Description = "Publisher for game",
            HomePageUrl = "https://old.com"
        };
        var publisher = await _pubService.CreatePublisher(createPublisher);
        var gameDto = new GameDto {
            Title = "Test Game", Description = "Test", Stock = 12, Price = 19.99m, ReleaseDate = DateTime.Now, PublisherId = publisher.Id,
            GenreNames = ["Strategy"], SubGenreNames = ["NonExistentSub"]
        };

        Assert.ThrowsAsync<GenresNotFoundException>(() => _gameService.CreateGameAsync(gameDto));
    }

    [Test]
    public async Task UpdateGame_ChangeGenres_Success() {
        var createPublisher = new CreatePublisherDTO
        {
            Name = "Initial Publisher",
            Description = "Publisher for game",
            HomePageUrl = "https://old.com"
        };
        var publisher = await _pubService.CreatePublisher(createPublisher);
        var existingGame = new Game {
            Title = "Existing Game", Description = "Test", Stock = 12, Price = 29.99m, PublisherId = publisher.Id,
            ReleaseDate = DateTime.Now, GenreIds = [1]
        };
        _context.Games.Add(existingGame);
        await _context.SaveChangesAsync();

        var updateDto = new GameDto {
            Title = "Existing Game", Description = "Test", Stock = 12, Price = 29.99m, ReleaseDate = DateTime.Now,
            GenreNames = ["Action"], SubGenreNames = ["FPS"],PublisherId = publisher.Id
        };

        var result = await _gameService.UpdateGameAsync(existingGame.Id, updateDto);

        Assert.That(result.GenreIds, Contains.Item(4)); // Action
        Assert.That(result.GenreIds, Contains.Item(5)); // FPS
        Assert.That(result.GenreIds, Does.Not.Contain(1)); // Strategy removed
    }

    [TearDown]
    public void TearDown() {
        _context.Dispose();
    }
}
