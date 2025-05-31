using EpamKse.GameStore.DataAccess.Repositories.Platform;
using EpamKse.GameStore.DataAccess.Repositories.Publisher;
using EpamKse.GameStore.Domain.Exceptions.Publisher;

namespace EpamKse.GameStore.Tests;

using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using EpamKse.GameStore.Services.Services.Publisher;
using EpamKse.GameStore.DataAccess.Context;
using EpamKse.GameStore.Domain.DTO.Publisher;
using EpamKse.GameStore.Domain.Exceptions;
using EpamKse.GameStore.Domain.Entities;

public class PublisherServiceTests
{
    private readonly GameStoreDbContext _dbContext;
    private readonly PublisherService _service;

    public PublisherServiceTests()
    {
        var options = new DbContextOptionsBuilder<GameStoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new GameStoreDbContext(options);
        var publisherRepository = new PublisherRepository(_dbContext);
        var platformRepository = new PlatformRepository(_dbContext);
        _service = new PublisherService(publisherRepository, platformRepository);
    }

    [Fact]
    public async Task CreatePublisher_SuccessfullyCreates()
    {
        var dto = new CreatePublisherDTO
        {
            Name = "New Publisher",
            Description = "Great games",
            HomePageUrl = "https://example.com"
        };

        var result = await _service.CreatePublisher(dto);

        Assert.Equal("New Publisher", result.Name);
    }

    [Fact]
    public async Task CreatePublisher_ThrowsConflict_WhenNameExists()
    {
        await _dbContext.Publishers.AddAsync(new Publisher
            { Name = "Existing", Description = "description", HomePageUrl = "https://example.com" });
        await _dbContext.SaveChangesAsync();

        var dto = new CreatePublisherDTO
        {
            Name = "Existing",
            Description = "Great games",
            HomePageUrl = "https://example.com"
        };

        await Assert.ThrowsAsync<PublisherDuplicationException>(() => _service.CreatePublisher(dto));
    }

    [Fact]
    public async Task DeletePublisher_DeletesExisting()
    {
        var publisher = new Publisher
            { Name = "toDelete", Description = "description", HomePageUrl = "https://example.com" };
        await _dbContext.Publishers.AddAsync(publisher);
        await _dbContext.SaveChangesAsync();

        await _service.DeletePublisher(publisher.Id);

        var deleted = await _dbContext.Publishers.FindAsync(publisher.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeletePublisher_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<PublisherNotFoundException>(() => _service.DeletePublisher(999));
    }

    [Fact]
    public async Task AddPlatformToPublisher_AddsLink()
    {
        var publisher = new Publisher
            { Name = "Test", Description = "description", HomePageUrl = "https://example.com" };
        var platform = new Platform { Name = "Windows" };
        await _dbContext.Publishers.AddAsync(publisher);
        await _dbContext.Platforms.AddAsync(platform);
        await _dbContext.SaveChangesAsync();

        await _service.AddPlatformToPublisher(new PlatformToPublisherDTO
        {
            Id = publisher.Id,
            platformId = platform.Id
        });

        var updated = await _dbContext.Publishers
            .Include(p => p.PublisherPlatforms)
            .FirstAsync(p => p.Id == publisher.Id);

        Assert.Contains(updated.PublisherPlatforms, p => p.Id == platform.Id);
    }

    [Fact]
    public async Task RemovePlatformFromPublisher_RemovesLink()
    {
        var platform = new Platform { Name = "Windows" };
        var publisher = new Publisher
            { Name = "Test", Description = "description", HomePageUrl = "https://example.com" };


        await _dbContext.Platforms.AddAsync(platform);
        await _dbContext.Publishers.AddAsync(publisher);
        await _dbContext.SaveChangesAsync();
        await _service.AddPlatformToPublisher(new PlatformToPublisherDTO
        {
            Id = publisher.Id,
            platformId = platform.Id
        });
        await _service.RemovePlatformFromPublisher(new PlatformToPublisherDTO
        {
            Id = publisher.Id,
            platformId = platform.Id
        });

        var updated = await _dbContext.Publishers
            .Include(p => p.PublisherPlatforms)
            .FirstAsync(p => p.Id == publisher.Id);

        Assert.DoesNotContain(updated.PublisherPlatforms, p => p.Id == platform.Id);
    }


    [Fact]
    public async Task UpdatePublisher_ValidUpdate_Succeeds()
    {
        var publisher = new Publisher
        {
            Name = "Original",
            Description = "Old Desc",
            HomePageUrl = "https://url.com",
            CreatedAt = DateTime.UtcNow
        };
        await _dbContext.Publishers.AddAsync(publisher);
        await _dbContext.SaveChangesAsync();

        var dto = new UpdatePublisherDTO
        {
            Id = publisher.Id,
            Name = "Updated",
            Description = "New Desc",
            HomePageUrl = "https://url.com"
        };

        var updated = await _service.UpdatePublisher(dto);

        Assert.Equal("Updated", updated.Name);
        Assert.Equal("New Desc", updated.Description);
        Assert.Equal("https://url.com", updated.HomePageUrl);
    }

    [Fact]
    public async Task UpdatePublisher_NotFound_Throws()
    {
        var dto = new UpdatePublisherDTO
        {
            Id = 999,
            Name = "DoesNotExist",
            Description = "desc",
            HomePageUrl = "https://url.com"
        };

        await Assert.ThrowsAsync<PublisherNotFoundException>(() => _service.UpdatePublisher(dto));
    }


    [Fact]
    public async Task GetPaginatedFullPublishers_ReturnsPaginatedList()
    {
        var publisher1 = new Publisher
        {
            Name = "A", Description = "desc",
            HomePageUrl = "https://url.com"
        };
        var publisher2 = new Publisher
        {
            Name = "B", Description = "desc",
            HomePageUrl = "https://url.com"
        };
        await _dbContext.Publishers.AddRangeAsync(publisher1, publisher2);
        await _dbContext.SaveChangesAsync();

        var result = await _service.GetPaginatedFullPublishers(page: 2, limit: 1);

        Assert.Single(result);
        Assert.Equal("B", result[0].Name);
    }

    [Fact]
    public async Task GetPublisherByIdWithFullData_NotFound_Throws()
    {
        await Assert.ThrowsAsync<PublisherNotFoundException>(() => _service.GetPublisherByIdWithFullData(123456));
    }

    [Fact]
    public async Task GetPublisherByIdWithFullData_ReturnsBasicPublisher()
    {
        var publisher = new Publisher
        {
            Name = "Test",
            Description = "desc",
            HomePageUrl = "https://url.com",
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Publishers.AddAsync(publisher);
        await _dbContext.SaveChangesAsync();

        var result = await _service.GetPublisherByIdWithFullData(publisher.Id);

        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
        Assert.Empty(result.Games);
        Assert.Empty(result.Platforms);
    }
}