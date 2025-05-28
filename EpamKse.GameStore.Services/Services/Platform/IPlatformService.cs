namespace EpamKse.GameStore.Services.Services.Platform;

using Domain.DTO.Platform;
using Domain.Entities;

public interface IPlatformService
{
    public Task<List<Platform>> GetPlatforms();
    public Task<Platform> GetPlatformById(int id);
    public Task<Platform> GetPlatformByName(string name);
    public Task<Platform> CreatePlatform(CreatePlatformDto dto);
    public Task<Platform> UpdatePlatform(int id, UpdatePlatformDto dto);
    public Task<Platform> DeletePlatform(int id);
}
