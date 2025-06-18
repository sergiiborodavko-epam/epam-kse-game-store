using System.Text.Json;
using EpamKse.GameStore.DataAccess.Repositories.License;
using EpamKse.GameStore.DataAccess.Repositories.Order;
using EpamKse.GameStore.Domain.DTO.License;
using EpamKse.GameStore.Domain.Entities;
using EpamKse.GameStore.Domain.Exceptions.License;
using EpamKse.GameStore.Domain.Exceptions.Order;
using EpamKse.GameStore.Services.Services.Encryption;
using EpamKse.GameStore.Services.Services.License;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace EpamKse.GameStore.Tests;

public class LicenseServiceTests
{
    private readonly Mock<ILicenseRepository> _licenseRepositoryMock;
    private readonly Mock<IEncryptionService> _encryptionServiceMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ILicenseBuilder> _licenseBuilderMock;
    private readonly LicenseService _licenseService;

    public LicenseServiceTests()
    {
        _licenseRepositoryMock = new Mock<ILicenseRepository>();
        _encryptionServiceMock = new Mock<IEncryptionService>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _licenseBuilderMock = new Mock<ILicenseBuilder>();
        _licenseService = new LicenseService(_licenseRepositoryMock.Object, _encryptionServiceMock.Object, _orderRepositoryMock.Object, _licenseBuilderMock.Object);;
    }

    [Fact]
    public void VerifyLicense_ReturnsLicenseKey_WhenKeyIsValid()
    {
        var key = "valid_key";
        var licenseKey = new LicenseKey { OrderId = 1, GameIds = new List<int> { 1, 2 }, UserId = 1 };
        var decryptedJson = JsonSerializer.Serialize(licenseKey);
        _encryptionServiceMock.Setup(s => s.Decrypt(key)).Returns(decryptedJson);

        var result = _licenseService.VerifyLicense(key);

        Assert.Equal(licenseKey.OrderId, result.OrderId);
        Assert.Equal(licenseKey.GameIds, result.GameIds);
        Assert.Equal(licenseKey.UserId, result.UserId);
    }

    [Fact]
    public void VerifyLicense_ThrowsInvalidLicenseKeyException_WhenDecryptionFails()
    {
        var key = "invalid_key";
        _encryptionServiceMock.Setup(s => s.Decrypt(key)).Throws<FormatException>();

        Assert.Throws<InvalidLicenseKeyException>(() => _licenseService.VerifyLicense(key));
    }

    [Fact]
    public async Task GenerateLicenseFileByOrderId_ReturnsBytes_WhenLicenseExists()
    {
        var orderId = 1;
        var license = new License
        {
            Id = 1,
            Key = "test_key",
            Order = new Order
            {
                Id = orderId,
                User = new User { FullName = "Test User", Email = "test@example.com" },
                Games = new List<Game> { new() { Title = "Game 1", Price = 10 } },
                TotalSum = 10
            }
        };
        var expectedBytes = new byte[] { 1, 2, 3 };
        
        _licenseRepositoryMock.Setup(r => r.GetByOrderIdAsync(orderId)).ReturnsAsync(license);
        _licenseBuilderMock.Setup(b => b.Build(license)).Returns(expectedBytes);

        var result = await _licenseService.GenerateLicenseFileByOrderId(orderId);

        Assert.Equal(expectedBytes, result);
    }

    [Fact]
    public async Task GenerateLicenseFileByOrderId_ThrowsLicenseNotFoundException_WhenLicenseDoesNotExist()
    {
        var orderId = 1;
        _licenseRepositoryMock.Setup(r => r.GetByOrderIdAsync(orderId)).ReturnsAsync((License)null);

        await Assert.ThrowsAsync<LicenseNotFoundException>(() => 
            _licenseService.GenerateLicenseFileByOrderId(orderId));
    }

    [Fact]
    public async Task GenerateLicenseFileByGameId_ReturnsBytes_WhenLicenseExists()
    {
        var userId = 1;
        var gameId = 1;
        var license = new License
        {
            Id = 1,
            Key = "test_key",
            Order = new Order
            {
                Id = 1,
                User = new User { FullName = "Test User", Email = "test@example.com" },
                Games = new List<Game> { new() { Id = gameId, Title = "Game 1", Price = 10 } },
                TotalSum = 10
            }
        };
        var expectedBytes = new byte[] { 1, 2, 3 };
        
        _licenseRepositoryMock.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<License> { license });
        _licenseBuilderMock.Setup(b => b.Build(license)).Returns(expectedBytes);

        var result = await _licenseService.GenerateLicenseFileByGameId(userId, gameId);

        Assert.Equal(expectedBytes, result);
    }

    [Fact]
    public async Task GenerateLicenseFileByGameId_ThrowsLicenseNotFoundException_WhenLicenseDoesNotExist()
    {
        var userId = 1;
        var gameId = 1;
        _licenseRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(new List<License>());

        await Assert.ThrowsAsync<LicenseNotFoundException>(() => 
            _licenseService.GenerateLicenseFileByGameId(userId, gameId));
    }

    [Fact]
    public async Task CreateLicense_ReturnsLicense_WhenOrderExistsAndLicenseDoesNotExist()
    {
        var orderId = 1;
        var order = new Order
        {
            Id = orderId,
            Games = new List<Game> { new() { Id = 1 } },
            UserId = 1
        };
        var createLicenseDto = new CreateLicenseDto { OrderId = orderId };
        var encryptedKey = "encrypted_key";

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
        _licenseRepositoryMock.Setup(r => r.GetByOrderIdAsync(orderId)).ReturnsAsync((License)null);
        _encryptionServiceMock.Setup(s => s.Encrypt(It.IsAny<string>())).Returns(encryptedKey);

        var result = await _licenseService.CreateLicense(createLicenseDto);

        Assert.NotNull(result);
        Assert.Equal(encryptedKey, result.Key);
        Assert.Equal(order, result.Order);
    }

    [Fact]
    public async Task CreateLicense_ThrowsOrderNotFoundException_WhenOrderDoesNotExist()
    {
        var orderId = 1;
        var createLicenseDto = new CreateLicenseDto { OrderId = orderId };
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order)null);

        await Assert.ThrowsAsync<OrderNotFoundException>(() => 
            _licenseService.CreateLicense(createLicenseDto));
    }

    [Fact]
    public async Task CreateLicense_ThrowsLicenseAlreadyExistsException_WhenLicenseExists()
    {
        var orderId = 1;
        var createLicenseDto = new CreateLicenseDto { OrderId = orderId };
        var order = new Order { Id = orderId };
        var existingLicense = new License { Order = order };

        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
        _licenseRepositoryMock.Setup(r => r.GetByOrderIdAsync(orderId)).ReturnsAsync(existingLicense);

        await Assert.ThrowsAsync<LicenseAlreadyExistsException>(() => 
            _licenseService.CreateLicense(createLicenseDto));
    }
}