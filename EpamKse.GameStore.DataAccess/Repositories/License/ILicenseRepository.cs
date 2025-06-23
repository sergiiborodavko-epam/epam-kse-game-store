namespace EpamKse.GameStore.DataAccess.Repositories.License;
using Domain.Entities;

public interface ILicenseRepository
{
    Task<IEnumerable<License>> GetAllAsync();
    Task<IEnumerable<License>> GetByUserIdAsync(int userId);
    Task<License?> GetByIdAsync(int id);
    Task<License?> GetByOrderIdAsync(int userId);
    Task<License?> GetByKeyAsync(string key);
    Task<License> CreateAsync(License license);
    Task DeleteAsync(License license);
}