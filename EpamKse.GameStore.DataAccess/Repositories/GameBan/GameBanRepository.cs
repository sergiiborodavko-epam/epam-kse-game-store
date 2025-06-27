namespace EpamKse.GameStore.DataAccess.Repositories.GameBan;

using Microsoft.EntityFrameworkCore;

using Context;
using Domain.Entities;
using Domain.Enums;

public class GameBanRepository(GameStoreDbContext context) : IGameBanRepository {
    public async Task<IEnumerable<GameBan>> GetAllAsync() {
        return await context.GameBans
            .Include(b => b.Game)
            .ToListAsync();
    }

    public async Task<GameBan?> GetByIdAsync(int id)
    {
        return await context.GameBans
            .Include(b => b.Game)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<GameBan?> GetByGameAndCountryAsync(int gameId, Countries country)
    {
        return await context.GameBans
            .FirstOrDefaultAsync(b => b.GameId == gameId && b.Country == country);
    }

    public async Task<IEnumerable<GameBan>> GetByCountryAsync(Countries country)
    {
        return await context.GameBans
            .Include(b => b.Game)
            .Where(b => b.Country == country)
            .ToListAsync();
    }

    public async Task<GameBan> CreateAsync(GameBan ban)
    {
        context.GameBans.Add(ban);
        await context.SaveChangesAsync();
        return ban;
    }

    public async Task DeleteAsync(GameBan ban)
    {
        context.GameBans.Remove(ban);
        await context.SaveChangesAsync();
    }
}
