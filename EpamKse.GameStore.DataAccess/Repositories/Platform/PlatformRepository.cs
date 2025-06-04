using EpamKse.GameStore.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace EpamKse.GameStore.DataAccess.Repositories.Platform;

using Domain.Entities;

public class PlatformRepository : IPlatformRepository
{
    private readonly GameStoreDbContext _dbContext;

    public PlatformRepository(GameStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Platform?> GetByIdAsync(int id)
    {
        return await _dbContext.Platforms.FindAsync(id);
    }

    public async Task<bool> IsPlatformLinkedToPublisherAsync(int publisherId, int platformId)
    {
        var publisher = await _dbContext.Publishers
            .Include(p => p.PublisherPlatforms)
            .FirstOrDefaultAsync(p => p.Id == publisherId);

        return publisher?.PublisherPlatforms.Any(p => p.Id == platformId) ?? false;
    }

    public async Task AddPlatformToPublisherAsync(Publisher publisher, Platform platform)
    {
        publisher.PublisherPlatforms.Add(platform);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemovePlatformFromPublisherAsync(Publisher publisher, Platform platform)
    {
        publisher.PublisherPlatforms.Remove(platform);
        await _dbContext.SaveChangesAsync();
    }
}