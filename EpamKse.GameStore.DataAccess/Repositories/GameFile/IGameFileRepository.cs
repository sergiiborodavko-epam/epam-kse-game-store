namespace EpamKse.GameStore.DataAccess.Repositories.GameFile;

using Domain.Entities;

public interface IGameFileRepository {
    Task<IEnumerable<GameFile>> GetAllAsync();
    Task<GameFile?> GetByIdAsync(int id);
    Task<GameFile?> GetByGameAndPlatformAsync(int gameId, int platformId);
    Task<IEnumerable<GameFile>> GetByGameIdAsync(int gameId);
    Task<GameFile> CreateAsync(GameFile gameFile);
    Task<GameFile> UpdateAsync(GameFile gameFile);
    Task DeleteAsync(GameFile gameFile);
}
