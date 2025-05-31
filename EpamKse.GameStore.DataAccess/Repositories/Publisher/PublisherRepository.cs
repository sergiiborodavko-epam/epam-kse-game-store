using EpamKse.GameStore.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace EpamKse.GameStore.DataAccess.Repositories.Publisher;

using Domain.Entities;

public class PublisherRepository : IPublisherRepository
{
    private readonly GameStoreDbContext _dbContext;

    public PublisherRepository(GameStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Publisher?> GetByIdAsync(int id)
    {
        return await _dbContext.Publishers
            .Include(p => p.Games)
            .ThenInclude(g => g.Platforms)
            .Include(p => p.PublisherPlatforms)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Publisher?> GetByNameAsync(string name)
    {
        return await _dbContext.Publishers
            .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
    }

    public async Task<List<Publisher>> GetPaginatedFullAsync(int skip, int take)
    {
        return await _dbContext.Publishers
            .Include(p => p.Games).ThenInclude(g => g.Platforms)
            .Include(p => p.PublisherPlatforms)
            .OrderBy(p => p.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<Publisher> AddAsync(Publisher publisher)
    {
        var entry = await _dbContext.Publishers.AddAsync(publisher);
        await _dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task RemoveAsync(Publisher publisher)
    {
        _dbContext.Publishers.Remove(publisher);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Publisher> UpdateAsync(Publisher publisher)
    {
        _dbContext.Publishers.Update(publisher);
        await _dbContext.SaveChangesAsync();
        return publisher;
    }
}