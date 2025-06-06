using EpamKse.GameStore.Domain.Exceptions.HistoricalPrice;
using Microsoft.EntityFrameworkCore;

namespace EpamKse.GameStore.DataAccess.Repositories.HistoricalPrice;

using EpamKse.GameStore.DataAccess.Context;
using Domain.Entities;

public class HistoricalPriceRepository : IHistoricalPriceRepository
{
    private readonly GameStoreDbContext _dbContext;

    public HistoricalPriceRepository(GameStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateHistoricalPrice(decimal price, int gameId)
    {
        if (price <= 0)
        {
            throw new PriceMustBeGreaterThenZeroException();
        }

        _dbContext.HistoricalPrices.Add(new HistoricalPrice
        {
            Price = price,
            GameId = gameId
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<HistoricalPrice>> GetPaginatedHistoricalPrices(int id, int skip, int take)
    {
        return await _dbContext.HistoricalPrices
            .Where(p => p.GameId == id)
            .OrderBy(p => p.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}