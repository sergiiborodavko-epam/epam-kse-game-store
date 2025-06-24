using EpamKse.GameStore.Domain.DTO.License;

namespace EpamKse.GameStore.Services.Services.License;
using Domain.Entities;

public interface ILicenseService
{
    LicenseKey VerifyLicense(string key);
    Task<byte[]> GenerateLicenseFileByOrderId(int orderId);
    Task<byte[]> GenerateLicenseFileByGameId(int userId, int gameId);
    Task<License> CreateLicense(CreateLicenseDto dto);
}