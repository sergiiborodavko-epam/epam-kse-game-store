namespace EpamKse.GameStore.DataAccess.Repositories.GameBan;

using Domain.Enums;
using Domain.Entities;

public interface IGameBanRepository {
    Task<IEnumerable<GameBan>> GetAllAsync();
    Task<GameBan?> GetByIdAsync(int id);
    Task<GameBan?> GetByGameAndCountryAsync(int gameId, Countries country);
    Task<IEnumerable<GameBan>> GetByCountryAsync(Countries country);
    Task<GameBan> CreateAsync(GameBan ban);
    Task DeleteAsync(GameBan ban);
}
