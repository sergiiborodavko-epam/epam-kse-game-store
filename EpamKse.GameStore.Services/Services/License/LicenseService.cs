using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EpamKse.GameStore.DataAccess.Repositories.License;
using EpamKse.GameStore.DataAccess.Repositories.Order;
using EpamKse.GameStore.Domain.DTO.License;
using EpamKse.GameStore.Domain.Exceptions.License;
using EpamKse.GameStore.Domain.Exceptions.Order;
using EpamKse.GameStore.Services.Services.Encryption;

namespace EpamKse.GameStore.Services.Services.License;
using Domain.Entities;

public class LicenseService : ILicenseService
{
    private readonly ILicenseRepository _licenseRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly IOrderRepository _orderRepository;
    private readonly ILicenseBuilder _licenseBuilder;
    
    public LicenseService(ILicenseRepository licenseRepository, IEncryptionService encryptionService, IOrderRepository orderRepository, ILicenseBuilder licenseBuilder)
    {
        _licenseRepository = licenseRepository;
        _encryptionService = encryptionService;
        _orderRepository = orderRepository;
        _licenseBuilder = licenseBuilder;
    }

    public LicenseKey VerifyLicense(string key)
    {
        try
        {
            var decryptedString = _encryptionService.Decrypt(key);
            return JsonSerializer.Deserialize<LicenseKey>(decryptedString);
        }
        catch (FormatException)
        {
            throw new InvalidLicenseKeyException(key);
        }
        catch (CryptographicException)
        {
            throw new InvalidLicenseKeyException(key);
        }
    }

    public async Task<byte[]> GenerateLicenseFileByOrderId(int orderId)
    {
        var license = await _licenseRepository.GetByOrderIdAsync(orderId);
        
        if (license == null)
        {
            throw new LicenseNotFoundException($"No license for order id {orderId}");
        }

        return _licenseBuilder.Build(license);
    }
    
    public async Task<byte[]> GenerateLicenseFileByGameId(int userId, int gameId)
    {
        var licenses = await _licenseRepository.GetByUserIdAsync(userId);
        var license = licenses.FirstOrDefault(l => l.Order.Games.Any(g => g.Id == gameId));
        
        if (license == null)
        {
            throw new LicenseNotFoundException($"No license for game id {gameId}");
        }

        return _licenseBuilder.Build(license);   
    }

    public async Task<License> CreateLicense(CreateLicenseDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(dto.OrderId!.Value);
        if (order == null)
        {
            throw new OrderNotFoundException(dto.OrderId!.Value);
        }

        var existingLicense = await _licenseRepository.GetByOrderIdAsync(dto.OrderId!.Value);
        if (existingLicense != null)
        {
            throw new LicenseAlreadyExistsException(dto.OrderId!.Value);       
        }
        
        var rawLicenseKey = new LicenseKey
        {
            OrderId = order.Id,
            GameIds = order.Games.Select(g => g.Id).ToList(),
            UserId = order.UserId
        };
        
        var licenseKeyJson = JsonSerializer.Serialize(rawLicenseKey);
        var licenseKey = _encryptionService.Encrypt(licenseKeyJson);

        var license = new License
        {
            Order = order,
            Key = licenseKey
        };
        
        await _licenseRepository.CreateAsync(license);
        return license;
    }
}