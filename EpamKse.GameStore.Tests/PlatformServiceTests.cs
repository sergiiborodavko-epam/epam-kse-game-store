using EpamKse.GameStore.Api.DTO.Platform;
using EpamKse.GameStore.Api.Services;
using EpamKse.GameStore.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace EpamKse.GameStore.Tests;

public class PlatformServiceTests
{
    private GameStoreDbContext _contextMock;
    private PlatformService _platformService;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<GameStoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _contextMock = new GameStoreDbContext(options);
        _contextMock.Database.EnsureCreated();

        _contextMock.SaveChanges();
        
        _platformService = new PlatformService(_contextMock);
    }

    [Test]
    public async Task GetPlatforms_ReturnsAllPlatforms()
    {
        var platforms = await _platformService.GetPlatforms();
        Assert.That(platforms, Is.Not.Null);
        Assert.That(platforms.Count, Is.EqualTo(_contextMock.Platforms.Count()));
    }

    [Test]
    public async Task GetPlatformByName_ReturnsPlatform_WhenFound()
    {
        var existingName = _contextMock.Platforms.First().Name;
        var platform = await _platformService.GetPlatformByName(existingName);
        
        Assert.That(platform, Is.Not.Null);
        Assert.That(platform.Name, Is.EqualTo(existingName));
    }

    [Test]
    public async Task GetPlatformByName_ReturnsNull_WhenNotFound()
    {
        var randomName = "dfjdjfdfjd";
        var platform = await _platformService.GetPlatformByName(randomName);
        
        Assert.That(platform, Is.Null);
    }

    [Test]
    public async Task GetPlatformById_ReturnsPlatform_WhenFound()
    {
        var existingId = _contextMock.Platforms.First().Id;
        var platform = await _platformService.GetPlatformById(existingId);
        
        Assert.That(platform, Is.Not.Null);
        Assert.That(platform.Id, Is.EqualTo(existingId));
    }

    [Test]
    public async Task GetPlatformById_ReturnsNull_WhenNotFound()
    {
        var randomId = 9999;
        var platform = await _platformService.GetPlatformById(randomId);
        
        Assert.That(platform, Is.Null);
    }

    [Test]
    public async Task CreatePlatform_ReturnsNewPlatform_WhenPlatformDoesNotExist()
    {
        var newPlatformDto = new CreatePlatformDto() { Name = "dfldkjfdjfkd" };
        var platform = await _platformService.CreatePlatform(newPlatformDto);
        
        Assert.That(platform.Name, Is.EqualTo(newPlatformDto.Name));
    }

    [Test]
    public async Task CreatePlatform_ReturnsNull_WhenPlatformExists()
    {
        var existingPlatformName = _contextMock.Platforms.First().Name;
        var newPlatformDto = new CreatePlatformDto() { Name = existingPlatformName };
        
        var platform = await _platformService.CreatePlatform(newPlatformDto);
        Assert.That(platform, Is.Null);
    }

    [Test]
    public async Task UpdatePlatform_ReturnsUpdatedPlatform_WhenFound()
    {
        var existingId = _contextMock.Platforms.First().Id;
        var newName = "test os name";
        
        var updatedPlatform = await _platformService.UpdatePlatform(existingId, new UpdatePlatformDto { Name = newName });
        
        Assert.That(existingId, Is.EqualTo(updatedPlatform.Id));
        Assert.That(newName, Is.EqualTo(updatedPlatform.Name));
    }

    [Test]
    public async Task UpdatePlatform_ReturnsNull_WhenNotFound()
    {
        var randomId = 9999;
        var platform = await _platformService.UpdatePlatform(randomId, new UpdatePlatformDto { Name = "test os name "});
        
        Assert.That(platform, Is.Null);
    }

    [Test]
    public async Task DeletePlatform_ReturnsDeletedPlatform_WhenFound()
    {
        var existingId = _contextMock.Platforms.First().Id;
        var deletedPlatform = await _platformService.DeletePlatform(existingId);
        
        Assert.That(deletedPlatform.Id, Is.EqualTo(existingId));
        Assert.That(_contextMock.Platforms.FirstOrDefault(platform => platform.Id == existingId), Is.Null);
    }

    [Test]
    public async Task DeletePlatform_ReturnsNull_WhenNotFound()
    {
        var randomId = 9999;
        var platform = await _platformService.DeletePlatform(randomId);
        
        Assert.That(platform, Is.Null);
    }
    
    [TearDown]
    public void TearDown()
    {
        _contextMock.Dispose();
    }
}