namespace EpamKse.GameStore.DataAccess.Repositories.GameFile;

using Microsoft.EntityFrameworkCore;
using Context;
using Domain.Entities;

public class GameFileRepository(GameStoreDbContext context) : IGameFileRepository {
    public async Task<IEnumerable<GameFile>> GetAllAsync() {
        return await context.GameFiles.ToListAsync();
    }

    public async Task<GameFile?> GetByIdAsync(int id) {
        return await context.GameFiles.FindAsync(id);
    }

    public async Task<GameFile?> GetByGameAndPlatformAsync(int gameId, int platformId) {
        return await context.GameFiles
            .FirstOrDefaultAsync(gf => gf.GameId == gameId && gf.PlatformId == platformId);
    }

    public async Task<IEnumerable<GameFile>> GetByGameIdAsync(int gameId) {
        return await context.GameFiles
            .Where(gf => gf.GameId == gameId)
            .ToListAsync();
    }

    public async Task<GameFile> CreateAsync(GameFile gameFile) {
        context.GameFiles.Add(gameFile);
        await context.SaveChangesAsync();
        return gameFile;
    }

    public async Task<GameFile> UpdateAsync(GameFile gameFile) {
        context.GameFiles.Update(gameFile);
        await context.SaveChangesAsync();
        return gameFile;
    }

    public async Task DeleteAsync(GameFile gameFile) {
        context.GameFiles.Remove(gameFile);
        await context.SaveChangesAsync();
    }
}
