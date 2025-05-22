namespace EpamKse.GameStore.DataAccess.Repositories.Game;

using Microsoft.EntityFrameworkCore;

using Context;
using Domain.Entities;

public class GameRepository(GameStoreDbContext context) : IGameRepository {

    public async Task<IEnumerable<Game>> GetAllAsync() {
        return await context.Games.ToListAsync();
    }

    public async Task<Game?> GetByIdAsync(int id) {
        return await context.Games.FindAsync(id);
    }
    
    public async Task<Game?> GetByTitleAsync(string title) {
        return await context.Games.FirstOrDefaultAsync(g => g.Title == title);
    }

    public async Task<Game> CreateAsync(Game game) {
        context.Games.Add(game);
        await context.SaveChangesAsync();
        return game;
    }

    public async Task<Game> UpdateAsync(Game game) {
        context.Games.Update(game);
        await context.SaveChangesAsync();
        return game;
    }

    public async Task DeleteAsync(Game game) {
        context.Games.Remove(game);
        await context.SaveChangesAsync();
    }
}
