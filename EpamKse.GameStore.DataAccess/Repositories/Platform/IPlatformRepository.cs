namespace EpamKse.GameStore.DataAccess.Repositories.Platform;

using Domain.Entities;

public interface IPlatformRepository
{
    Task<Platform?> GetByIdAsync(int id);
    Task<bool> IsPlatformLinkedToPublisherAsync(int publisherId, int platformId);
    Task AddPlatformToPublisherAsync(Publisher publisher, Platform platform);
    Task RemovePlatformFromPublisherAsync(Publisher publisher, Platform platform);
}