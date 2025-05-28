namespace EpamKse.GameStore.DataAccess.Repositories.Game;

using Domain.Entities;

public interface IGameRepository {
    Task<IEnumerable<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(int id);
    Task<Game?> GetByTitleAsync(string title);
    Task<Game> CreateAsync(Game game);
    Task<Game> UpdateAsync(Game game);
    Task DeleteAsync(Game game);
}
